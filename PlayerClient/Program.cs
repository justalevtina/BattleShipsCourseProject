namespace PlayerClient
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConnectionForm connectionForm1 = new ConnectionForm();
            connectionForm1.Show();

            ConnectionForm connectionForm2 = new ConnectionForm();
            connectionForm2.Show();

            Application.Run();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            // ApplicationConfiguration.Initialize();
            // Application.Run(new ConnectionForm());
        }
    }
}