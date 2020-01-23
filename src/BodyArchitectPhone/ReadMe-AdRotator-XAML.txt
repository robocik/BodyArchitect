Welcome to AdRotator for XAML projects includng Windows 8 and Windows Phone Silverlight

Included in this package is:

* This Readme :D
* AdRotator DLL
* Sample "DefaultAdSetings.XML" configuration file

To implement this control add the following XAML to your project.

Add the namespace reference for the AdRotator Project to your page:

    xmlns:AdRotatorWP7="clr-namespace:AdRotator;assembly=AdRotator"

Then add the AdRotator control to your page:
(We recommend using a UserControl in your project for implementation especially if you intend to use it on several pages)

	<AdRotatorWP7:AdRotatorControl 
		x:Name="AdRotator"
		AdHeight="90"
		AdWidth="728"
		DefaultSettingsFileUri="/AdRotatorExample;component/defaultAdSettings.xml"
		SettingsUrl="http://adrotator.apphb.com/defaultAdSettingsWindows8dev.xml" />

Next you need to initialise the control by calling "Invalidate" in the "Loaded" event in the Page / User Control code behind:

        public MainPage()
        {
            InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AdRotator.Invalidate();
        }

Lastly configure the XML configuration file (optionally host it on the web to allow remote configuration) for your Ad Provider settings or alternatively set the providers in the XAML

For more instructions on how to implement this control and all the other configuration options checkout the AdRotator host site
http://wp7adrotator.codeplex.com

**Note - Default Ad implementation now uses a string instead of an object, it is also now alo available in the config XML as a secondary ID for DefaulAds.
         Format = <namespace>.<ClassName>

The AdRotator Team
Simon Jackson (http://darkgenesis.zenithmoon.com)
Gergely Orosz (http://gregdoesit.com)