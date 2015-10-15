namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class PopupButton : Button
    {
        public const string TextOverlayTemplateKey = "TextOverlayTemplateKey"; // { get { return "TextOverlayTemplateKey"; } }
        public const string DefaultOverlayTemplateKey = "DefaultOverlayTemplateKey"; //{ get { return "DefaultOverlayTemplateKey"; } }
        public const string MissingToolTipKey = "MissingToolTipKey"; // { get { return "MissingToolTipKey"; } }

        public static readonly DependencyProperty UseTouchToolTipAsMouseOverToolTipProperty = TouchToolTipService.UseTouchToolTipAsMouseOverToolTipProperty.AddOwner(
            typeof(PopupButton),
            new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.Inherits,
                OnUseTouchToolTipAsMousoverToolTipChanged));

        public static readonly DependencyProperty TouchToolTipProperty = DependencyProperty.Register(
            "TouchToolTip",
            typeof(ToolTip),
            typeof(PopupButton),
            new PropertyMetadata(
                default(ToolTip),
                OnTouchToolTipChanged));

        private static readonly DependencyProperty OriginalToolTipProperty = DependencyProperty.Register(
            "OriginalToolTip",
            typeof(ToolTip),
            typeof(PopupButton),
            new PropertyMetadata(default(ToolTip)));

        /// <summary>
        /// Used for binding touchtooltips UseTouchToolTipAsMousoverToolTip to get notified of changes.
        /// </summary>
        private static readonly DependencyProperty UseTouchToolTipAsMousoverToolTipProxyProperty = DependencyProperty.Register(
            "UseTouchToolTipAsMousoverToolTipProxy",
            typeof(bool),
            typeof(PopupButton),
            new PropertyMetadata(default(bool), OnUseTouchToolTipAsMousoverToolTipChanged));

        /// <summary>
        /// Used for binding ToolTip.IsOpen and TouchToolTip.IsOpen to update _lastChangeTime
        /// Internal for tests
        /// </summary>
        internal static readonly DependencyProperty IsOpensProperty = DependencyProperty.Register(
            "IsOpens",
            typeof(bool),
            typeof(PopupButton),
            new PropertyMetadata(false, OnIsOpensChanged));

        internal DateTimeOffset LastChangeTime { get; private set; } = DateTimeOffset.Now;

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));

        }

        public PopupButton()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(OnClick), true);
            AddHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocus), true);
            AddHandler(ToolTipService.ToolTipOpeningEvent, new RoutedEventHandler(OnToolTipOpening));
            IsVisibleChanged += OnIsVisibleChanged;

            var isOpensBinding = new MultiBinding
            {
                Converter = IsAnyTrueConverter.Default,
                Mode = BindingMode.OneWay
            };
            isOpensBinding.Bindings.Add(this.CreateBinding(ToolTipProperty, System.Windows.Controls.ToolTip.IsOpenProperty));
            isOpensBinding.Bindings.Add(this.CreateBinding(TouchToolTipProperty, System.Windows.Controls.ToolTip.IsOpenProperty));
            BindingOperations.SetBinding(this, IsOpensProperty, isOpensBinding);
        }

        public ToolTip TouchToolTip
        {
            get { return (ToolTip)GetValue(TouchToolTipProperty); }
            set { SetValue(TouchToolTipProperty, value); }
        }

        /// <summary>
        /// Null means it uses value from TouchToolTip
        /// </summary>
        public bool? UseTouchToolTipAsMouseOverToolTip
        {
            get { return (bool?)this.GetValue(UseTouchToolTipAsMouseOverToolTipProperty); }
            set { this.SetValue(UseTouchToolTipAsMouseOverToolTipProperty, value); }
        }

        protected virtual void OnTouchToolTipChanged(ToolTip oldToolTip, ToolTip newToolTip)
        {
            if (oldToolTip != null)
            {
                BindingOperations.ClearBinding(this, UseTouchToolTipAsMousoverToolTipProxyProperty);
                oldToolTip.PlacementTarget = null;
            }
            if (newToolTip != null)
            {
                var binding = new Binding
                {
                    Path = TouchToolTipService.UseTouchToolTipAsMouseOverToolTipProperty.AsPropertyPath(),
                    Source = newToolTip,
                    Mode = BindingMode.OneWay
                };
                BindingOperations.SetBinding(this, UseTouchToolTipAsMousoverToolTipProxyProperty, binding);
                newToolTip.PlacementTarget = this;
            }
        }

        private static void OnTouchToolTipChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((PopupButton)o).OnTouchToolTipChanged((ToolTip)e.OldValue, (ToolTip)e.NewValue);
        }

        private static void OnUseTouchToolTipAsMousoverToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool use;
            if (d.GetValue(UseTouchToolTipAsMouseOverToolTipProperty) == null)
            {
                use = d.GetValue(UseTouchToolTipAsMousoverToolTipProxyProperty) as bool? == true;
            }
            else
            {
                use = d.GetValue(UseTouchToolTipAsMouseOverToolTipProperty) as bool? == true;
            }
            if (use)
            {
                var toolTip = ToolTipService.GetToolTip(d);
                var touchTooltip = d.GetValue(TouchToolTipProperty);
                if (toolTip != null && !ReferenceEquals(toolTip, touchTooltip))
                {
                    d.SetValue(OriginalToolTipProperty, toolTip);
                }

                ToolTipService.SetToolTip(d, d.GetValue(TouchToolTipProperty));
            }
            else
            {
                ToolTipService.SetToolTip(d, d.GetValue(OriginalToolTipProperty));
            }
        }

        private static void OnIsOpensChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PopupButton)d).LastChangeTime = DateTimeOffset.Now;
        }

        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (DateTimeOffset.Now - LastChangeTime < TimeSpan.FromMilliseconds(200))
            {
                Debug.WriteLine("DateTimeOffset.Now - LastChangeTime < TimeSpan.FromMilliseconds(10)");
                return;
            }
            var toolTip = ToolTipService.GetToolTip(this) as ToolTip;
            if (toolTip != null && toolTip.IsOpen)
            {
                toolTip.IsOpen = false;
                return;
            }
            var touchToolTip = TouchToolTip;
            if (touchToolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }

            touchToolTip.IsOpen = !touchToolTip.IsOpen;
            Debug.WriteLine("Clicked: {0}, IsOpen: {1}", ((PopupButton)sender).Name, touchToolTip.IsOpen);
        }

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            //Debug.WriteLine("OnLostFocus");
            var touchToolTip = TouchToolTip;
            if (touchToolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }
            var close = touchToolTip.IsOpen && !(IsKeyboardFocusWithin || touchToolTip.IsKeyboardFocusWithin);
            Debug.Print(
                "{0}.LostFocus toolTip.IsOpen: {1} && (this.IsKeyboardFocusWithin: {2} || toolTip.IsKeyboardFocusWithin: {3}): {4}",
                ((PopupButton)sender).Name,
                touchToolTip.IsOpen,
                this.IsKeyboardFocusWithin,
                touchToolTip.IsKeyboardFocusWithin,
                close);

            if (close)
            {
                touchToolTip.IsOpen = false;
                Debug.WriteLine(touchToolTip.IsOpen);
            }
        }

        private void OnToolTipOpening(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (TouchToolTip != null)
            {
                TouchToolTip.IsOpen = false;
            }
        }

    }
}
