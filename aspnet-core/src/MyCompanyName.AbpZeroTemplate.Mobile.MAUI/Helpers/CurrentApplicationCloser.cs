namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Helpers
{
    public static class CurrentApplicationCloser
    {
        public static void Quit()
        {
#if IOS
        System.Environment.Exit(0); 
#else
            Application.Current.Quit();

#endif
        }
    }
}
