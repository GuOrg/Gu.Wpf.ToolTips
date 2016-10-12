# Gu.Wpf.ToolTips
Project for tooltips that works for touch users.

##### Simple sample:
```xaml
<AdornerDecorator>
    <!-- more content here -->
    <TextBlock HorizontalAlignment="Left"
               Text="Some text with toolip">
        <toolTips:TouchToolTipService.ToolTip>
            <ToolTip>
                <TextBlock Text="Tooltip text" />
            </ToolTip>
        </toolTips:TouchToolTipService.ToolTip>
    </TextBlock>
</AdornerDecorator>
```

##### Sample toggle visibility:
```xaml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="20" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <CheckBox x:Name="IsOverlayVisible"
              Content="IsOverlayVisible"
              IsChecked="True" />

    <TextBlock Grid.Row="2"
               HorizontalAlignment="Left"
               Text="Some text with toolip"
               toolTips:TouchToolTipService.IsOverlayVisible="{Binding IsChecked,
                                                                       ElementName=IsOverlayVisible}">
        <toolTips:TouchToolTipService.ToolTip>
            <ToolTip>
                <TextBlock Text="Tooltip text" />
            </ToolTip>
        </toolTips:TouchToolTipService.ToolTip>
    </TextBlock>
</Grid>
```

Renders:

![screenie](http://i.imgur.com/wbasIMg.gif)

##### How it works:
By setting `toolTips:TouchToolTipService.ToolTip` the controls tooltip is set and a transparent overlay with a button controlling tooltip visibility is added to the control.
For custom styling use a different style for `PopupButton` the default shows a blue circle with a white i hinting a touch user that there is a tooltip that can be toggled by tapping.
