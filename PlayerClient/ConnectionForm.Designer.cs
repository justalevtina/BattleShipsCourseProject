namespace PlayerClient
{
    partial class ConnectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ServerComboBox = new ComboBox();
            SuspendLayout();
            // 
            // ServerComboBox
            // 
            ServerComboBox.FormattingEnabled = true;
            ServerComboBox.Location = new Point(369, 190);
            ServerComboBox.Name = "ServerComboBox";
            ServerComboBox.Size = new Size(151, 28);
            ServerComboBox.TabIndex = 0;
            // 
            // ConnectionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(ServerComboBox);
            Name = "ConnectionForm";
            Text = "ConnectionForm";
            ResumeLayout(false);
        }

        #endregion

        private ComboBox ServerComboBox;
    }
}