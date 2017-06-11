The **UserArgs** class helps in defining the default configuration that the application will use when it is launched. Most of these features define the options available directly from the application using Options GUI editor. 

The LoadConfiguration is called when the application loads Configuration of the file. If you tweak this section of code, you can change the default behavior of the application. 

For instance, 

{{
private void LoadParameters()
        {
            this.Text = "Test Watermark";
            this.FontName = "Arial";
            this.FontSize = 20f;
            this.Color = "#ffffff";
            this.Opacity = 0.70f;
            this.IsItalic = false;
            this.IsBold = true;
            this.IsUnderLine = true;
            this.Left = 50;
            this.Top = 50;
        }
}}

This section loads the default options for Watermark that needs to be placed on Final image. 