   M S B A M L   `   `   `  ���� G  CeBirdLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA0clr-namespace:eBirdLibrary;assembly=eBirdLibraryeBirdLibrary  F BBirdTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:BirdTrackerBirdTracker    BirdTracker.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Ccustom0clr-namespace:eBirdLibrary;assembly=eBirdLibrary     1viewModelVmclr-namespace:BirdTracker     -   $
��main��  � Title$  Bird Tracker��$	��420��$	��866�� � 	Resources   BirdTracker.MainWindowVM   MainWindowVM��%   &	          .��� -   $��MainGrid��$
��Auto�� � DataContext# [   � RowDefinitions �� $��30 ��� $��* ��� $��30 �.+�c� $��0��.@��� $
��Auto��$��Report:�� �� TextAlignment$ Center=�$��Center=��� -   $��Report��$
��Auto��$	��100��$��Center=�����  �� Source# [   �� Path$ REPORT_TYPES� �� SelectedValue �� # [  $ SELECTED_REPORT��� $
��Auto��$��Center=�$��
Lattitude:���� -   $��	Lattitude��$
��Auto��$��Center=�$	��100������ # [  $ 	LATTITUDE��� $
��Auto��$��Center=�$��
Longitude:���� -   $��	Longitude��$
��Auto��$��Center=�$	��100������ # [  $ 	LONGITUDE��� $
��Auto��$��Center=�$��40������ # [  $ GENERATE_REPORT_CMD�.��Go�� $
��Auto��$��Center=�$��40������ # [  $ LAUNCH_MAP_CMD�.��Map�� $��1����. $
��Auto=�$��Disabled=�.���� -   $��DockingPanel�� �� Orientation$ 
Horizontal=����#555560�� $��2��