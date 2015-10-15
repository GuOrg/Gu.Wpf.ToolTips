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

        private static readonly PropertyPath UseTouchToolTipAsMousoverToolTipPath = new PropertyPath(TouchToolTipService.UseTouchToolTipAsMouseOverToolTipProperty);

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PopupButton),
                new FrameworkPropertyMetadata(typeof(PopupButton)));
        }

        public PopupButton()
        {
            AddHandler(ClickEvent, new RoutedEventHandler(OnClick), true);
            AddHandler(LostFocusEvent, new RoutedEventHandler(OnLostFocus), true);
            IsVisibleChanged += OnIsVisibleChanged;
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
                    Path = UseTouchToolTipAsMousoverToolTipPath,
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

        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var toolTip = TouchToolTip;
            if (toolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }
            toolTip.IsOpen = !toolTip.IsOpen;
            Debug.WriteLine("Clicked: {0}, IsOpen: {1}", ((PopupButton)sender).Name, toolTip.IsOpen);
        }

        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            //Debug.WriteLine("OnLostFocus");
            var toolTip = TouchToolTip;
            if (toolTip == null)
            {
                Debug.WriteLine("toolTip == null");
                return;
            }
            var close = toolTip.IsOpen && !(IsKeyboardFocusWithin || toolTip.IsKeyboardFocusWithin);
            Debug.Print(
                "{0}.LostFocus toolTip.IsOpen: {1} && (this.IsKeyboardFocusWithin: {2} || toolTip.IsKeyboardFocusWithin: {3}): {4}",
                ((PopupButton)sender).Name,
                toolTip.IsOpen,
                this.IsKeyboardFocusWithin,
                toolTip.IsKeyboardFocusWithin,
                close);

            if (close)
            {
                toolTip.IsOpen = false;
                Debug.WriteLine(toolTip.IsOpen);
            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TouchToolTip.IsOpen = false;
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
    }
}
