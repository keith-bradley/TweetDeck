   M S B A M L   `   `   `  ���� G  CeBirdLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA0clr-namespace:eBirdLibrary;assembly=eBirdLibraryeBirdLibrary  F BBirdTracker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:BirdTrackerBirdTracker 9!clr-namespace:BirdTracker.SupportBirdTracker.Support ,   &BirdTracker.GenericBirdSightingsReport   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Ccustom0clr-namespace:eBirdLibrary;assembly=eBirdLibrary     /	viewModelclr-namespace:BirdTracker     3local!clr-namespace:BirdTracker.Support     -   $��
ReportCtrl��$	��395��
������   � 	Resources  )  #BirdTracker.GenericSightingReportVM 
  BirdVM,  &BirdTracker.Support.NullImageConverter  nullImageConverter,  &BirdTracker.Species_Location_Converter  SLC��%-   &	        &	      &	 
           .���� ���#262830$��4,4,4,4q� �� CornerRadius$ 12,12,12,12��$��4,8,4,4q�._�� -    �Name$ MainGrid��$	��380�� � DataContext# [  ���#262830 � ColumnDefinitions �� $��* � � RowDefinitions �� $��20 ��� $��* �.+��� $	��380�� �� Orientation$ 
Horizontal=�$��0��.+��� $	��360�� �� TextAlignment$
 Left=����#262830
�������$
��Bold"�����  �� Path$ Report_Title��� $��20��$��x��
�������$
��Bold"�
������ ���� 	 �� Source#	 [  $ CLOSE_REPORT_CMD��� -   $��	lbReports��$	��380��$��1��$
��Left=�$��Stretch=����#262830$��0q����� $ ListSightings�
 �� Mode$
 TwoWay=�   �� Color��%�   (��       * SystemColors.HighlightBrushKey+)(��      *$"SystemColors.HighlightTextBrushKey+)(�� '     *SystemColors.ControlBrushKey+)�� $ Transparent���� $
 Gray���� $ Transparent������ .R���  ��Name$ border1��$ 23��$��1q����	#19000000$	��356��._��� $ border2��$ 22��$��1q����	#33000000$	��354��._��� $ border3��$ 21��$��1q����	#66000000$	��352��._��� $ border4��$ 20��$��1q����	#80000000
������� �� Padding$ 10q�$	��350��._��  �� $
��Auto ��� $��* �.+��� $��0��$��50��$��2,2,2,2q����� $ 	ThumbNail��� $��stkPanel��$��1��.+��  �� $
��Auto ��� $��* �.+��� $��
CommonName��$��0��$
��bold"����� $ _common_name��� $��ScientificName��$��1��$��normal"�$��Italic$�$��	Underline{����� $ _scientific_name� �� StringFormat$  ({0})�� J� InputBindings d�  d� MouseAction$ 	LeftClicke� �� Command �� #	 [  $ GET_SPECIES_REPORT_CMD� �� CommandParameter `�  `� 	Converter# [ .1���  �� ElementName$ 
CommonName��$
 Text��� $ ScientificName��$
 Text��� $��Normal"����� $ _location_name�$ Location: {0}���� $��Normal"����� $ _observation_date�$ Reported: {0}���� $��
LocationId��$��Normal"�$��	Underline{����� $ _location_id�$ Location ID: {0}�� d� $ 	LeftClicke� �� #	 [  $ GET_LOCATIONREPORT_CMD� �� *Text+$ 
LocationId��