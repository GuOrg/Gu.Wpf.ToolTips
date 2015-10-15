namespace Gu.Wpf.ToolTips.Tests
{
    using System.Windows.Controls;

    using NUnit.Framework;

    [RequiresSTA]
    public class PopupButtonTests
    {
        [Test]
        public void HandlesTouchToolUseTouchToolTipAsMouseOverToolTipChanges()
        {
            var popupButton = new PopupButton();
            var toolTip = new ToolTip();
            popupButton.ToolTip = toolTip;
            Assert.AreSame(toolTip, ToolTipService.GetToolTip(popupButton));
            var touchToolTip = new TouchToolTip();
            TouchToolTipService.SetUseTouchToolTipAsMouseOverToolTip(touchToolTip, false);
            popupButton.TouchToolTip = touchToolTip;
            Assert.AreSame(toolTip, ToolTipService.GetToolTip(popupButton));

            TouchToolTipService.SetUseTouchToolTipAsMouseOverToolTip(touchToolTip, true);
            Assert.AreSame(touchToolTip, ToolTipService.GetToolTip(popupButton));

            TouchToolTipService.SetUseTouchToolTipAsMouseOverToolTip(touchToolTip, false);
            Assert.AreSame(toolTip, ToolTipService.GetToolTip(popupButton));
        }

        [Test]
        public void HandlesUseTouchToolTipAsMouseOverToolTipChanges()
        {
            var popupButton = new PopupButton();
            var toolTip = new ToolTip();
            popupButton.ToolTip = toolTip;
            Assert.AreSame(toolTip, ToolTipService.GetToolTip(popupButton));
            var touchToolTip = new TouchToolTip();
            TouchToolTipService.SetUseTouchToolTipAsMouseOverToolTip(touchToolTip, false);
            popupButton.TouchToolTip = touchToolTip;
            Assert.AreSame(toolTip, ToolTipService.GetToolTip(popupButton));

            popupButton.UseTouchToolTipAsMouseOverToolTip = true;
            Assert.AreSame(touchToolTip, ToolTipService.GetToolTip(popupButton));

            TouchToolTipService.SetUseTouchToolTipAsMouseOverToolTip(touchToolTip, true);
            Assert.AreSame(touchToolTip, ToolTipService.GetToolTip(popupButton));

            TouchToolTipService.SetUseTouchToolTipAsMouseOverToolTip(touchToolTip, false);
            Assert.AreSame(touchToolTip, ToolTipService.GetToolTip(popupButton));

            popupButton.UseTouchToolTipAsMouseOverToolTip = null;
            Assert.AreSame(toolTip, ToolTipService.GetToolTip(popupButton));

            TouchToolTipService.SetUseTouchToolTipAsMouseOverToolTip(touchToolTip, true);
            Assert.AreSame(touchToolTip, ToolTipService.GetToolTip(popupButton));

            popupButton.UseTouchToolTipAsMouseOverToolTip = false;
            Assert.AreSame(toolTip, ToolTipService.GetToolTip(popupButton));
        }

        [Test]
        public void AddOwnerTest()
        {
            var popupButton = new PopupButton { UseTouchToolTipAsMouseOverToolTip = false };
            Assert.AreEqual(false, TouchToolTipService.GetUseTouchToolTipAsMouseOverToolTip(popupButton));

            popupButton.UseTouchToolTipAsMouseOverToolTip = true;
            Assert.AreEqual(true, TouchToolTipService.GetUseTouchToolTipAsMouseOverToolTip(popupButton));
        }
    }
}
