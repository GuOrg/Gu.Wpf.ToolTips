# Gu.Wpf.ToolTips
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE) 
[![NuGet](https://img.shields.io/nuget/v/Gu.Wpf.ToolTips.svg)](https://www.nuget.org/packages/Gu.Wpf.ToolTips/)
[![Build status](https://ci.appveyor.com/api/projects/status/j4myy99it0now2gv/branch/master?svg=true)](https://ci.appveyor.com/project/JohanLarsson/gu-wpf-tooltips/branch/master)
[![Build Status](https://dev.azure.com/guorg/Gu.Wpf.ToolTips/_apis/build/status/GuOrg.Gu.Wpf.ToolTips?branchName=master)](https://dev.azure.com/guorg/Gu.Wpf.ToolTips/_build/latest?definitionId=15&branchName=master)

Helpers for tool tips.

## TouchToolTipService
Tool tips for touch users. Setting `TouchToolTipService.Enabled="True"` on an element draws an OverlayAdorner over the element with an icon indicating that there is touch info.
Tapping opens/closes the tool tip.

#### Simple sample:
```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:toolTips="http://Gu.com/ToolTips"
        Title="DemoWindow"
        Width="100"
        SizeToContent="Height">
    <Window.Resources>
        <Style TargetType="{x:Type Button}">
            <Setter Property="Padding" Value="6" />
            <Setter Property="toolTips:TouchToolTipService.IsEnabled" Value="False" />
            <Setter Property="ToolTipService.ShowOnDisabled" Value="True" />
            <Style.Triggers>
                <Trigger Property="IsEnabled" Value="False">
                    <Setter Property="toolTips:TouchToolTipService.IsEnabled" Value="True" />
                </Trigger>
            </Style.Triggers>
        </Style>

        <Style TargetType="{x:Type TextBlock}">
            <Setter Property="toolTips:TouchToolTipService.IsEnabled" Value="True" />
            <Setter Property="Padding" Value="6" />
        </Style>

        <Style TargetType="{x:Type Label}">
            <Setter Property="toolTips:TouchToolTipService.IsEnabled" Value="True" />
            <Setter Property="Padding" Value="6" />
        </Style>
    </Window.Resources>

    <AdornerDecorator>
        <StackPanel>
            <Button Content="Button"
                IsEnabled="False"
                ToolTip="Button tool tip." />

            <TextBlock Text="TextBlock"
                   ToolTip="TextBlock tool tip." />

            <Label Content="Label"
                   ToolTip="Label tool tip." />
        </StackPanel>

    </AdornerDecorator>
</Window>
```
Renders:

![screenie](http://i.imgur.com/wbasIMg.gif)

#### OverlayAdorner

The default style for the overlay is:

```xaml
<Style TargetType="{x:Type local:OverlayAdorner}">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate>
                <Ellipse x:Name="TouchInfoIcon"
                            Height="{Binding RelativeSource={RelativeSource Self},
                                            Path=ActualWidth}"
                            Width="12"
                            Canvas.Top="0"
                            Canvas.Right="0"
                            Margin="0,2,2,0"
                            HorizontalAlignment="Right"
                            VerticalAlignment="Top">
                    <Ellipse.Fill>
                        <DrawingBrush>
                            <DrawingBrush.Drawing>
                                <DrawingGroup>
                                    <DrawingGroup.Children>
                                        <GeometryDrawing Brush="{DynamicResource {x:Static SystemColors.HighlightBrushKey}}">
                                            <GeometryDrawing.Geometry>
                                                <EllipseGeometry Center="50,50"
                                                                    RadiusX="50"
                                                                    RadiusY="50" />
                                            </GeometryDrawing.Geometry>
                                        </GeometryDrawing>

                                        <GeometryDrawing Brush="White">
                                            <GeometryDrawing.Geometry>
                                                <GeometryGroup>
                                                    <RectangleGeometry Rect="43,15,14,15" />
                                                    <RectangleGeometry Rect="43,36,14,50" />
                                                </GeometryGroup>
                                            </GeometryDrawing.Geometry>
                                        </GeometryDrawing>
                                    </DrawingGroup.Children>
                                </DrawingGroup>
                            </DrawingBrush.Drawing>
                        </DrawingBrush>
                    </Ellipse.Fill>
                </Ellipse>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```
