namespace KomunikatorKlient
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonRejestracja = new System.Windows.Forms.Button();
            this.buttonRozmowy = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonLogowanie = new System.Windows.Forms.Button();
            this.labelUserStatus = new System.Windows.Forms.Label();
            this.labelUserNumber = new System.Windows.Forms.Label();
            this.buttonUstawienia = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.buttonPolacz = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonRejestracja
            // 
            this.buttonRejestracja.Location = new System.Drawing.Point(247, 139);
            this.buttonRejestracja.Name = "buttonRejestracja";
            this.buttonRejestracja.Size = new System.Drawing.Size(125, 23);
            this.buttonRejestracja.TabIndex = 0;
            this.buttonRejestracja.Text = "Rejestracja";
            this.buttonRejestracja.UseVisualStyleBackColor = true;
            this.buttonRejestracja.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonRozmowy
            // 
            this.buttonRozmowy.Location = new System.Drawing.Point(130, 217);
            this.buttonRozmowy.Name = "buttonRozmowy";
            this.buttonRozmowy.Size = new System.Drawing.Size(125, 23);
            this.buttonRozmowy.TabIndex = 1;
            this.buttonRozmowy.Text = "Okno rozmowy";
            this.buttonRozmowy.UseVisualStyleBackColor = true;
            this.buttonRozmowy.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Status klienta:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Twój numer:";
            // 
            // buttonLogowanie
            // 
            this.buttonLogowanie.Location = new System.Drawing.Point(247, 107);
            this.buttonLogowanie.Name = "buttonLogowanie";
            this.buttonLogowanie.Size = new System.Drawing.Size(125, 23);
            this.buttonLogowanie.TabIndex = 4;
            this.buttonLogowanie.Text = "Zaloguj się";
            this.buttonLogowanie.UseVisualStyleBackColor = true;
            this.buttonLogowanie.Click += new System.EventHandler(this.buttonLogowanie_Click);
            // 
            // labelUserStatus
            // 
            this.labelUserStatus.AutoSize = true;
            this.labelUserStatus.Location = new System.Drawing.Point(92, 28);
            this.labelUserStatus.Name = "labelUserStatus";
            this.labelUserStatus.Size = new System.Drawing.Size(35, 13);
            this.labelUserStatus.TabIndex = 5;
            this.labelUserStatus.Text = "label3";
            // 
            // labelUserNumber
            // 
            this.labelUserNumber.AutoSize = true;
            this.labelUserNumber.Location = new System.Drawing.Point(82, 47);
            this.labelUserNumber.Name = "labelUserNumber";
            this.labelUserNumber.Size = new System.Drawing.Size(35, 13);
            this.labelUserNumber.TabIndex = 6;
            this.labelUserNumber.Text = "label4";
            // 
            // buttonUstawienia
            // 
            this.buttonUstawienia.Location = new System.Drawing.Point(247, 9);
            this.buttonUstawienia.Name = "buttonUstawienia";
            this.buttonUstawienia.Size = new System.Drawing.Size(125, 23);
            this.buttonUstawienia.TabIndex = 7;
            this.buttonUstawienia.Text = "Ustawienia";
            this.buttonUstawienia.UseVisualStyleBackColor = true;
            this.buttonUstawienia.Click += new System.EventHandler(this.button4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Połączenie z serwerem:";
            // 
            // labelConnectionStatus
            // 
            this.labelConnectionStatus.AutoSize = true;
            this.labelConnectionStatus.Location = new System.Drawing.Point(138, 9);
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            this.labelConnectionStatus.Size = new System.Drawing.Size(35, 13);
            this.labelConnectionStatus.TabIndex = 9;
            this.labelConnectionStatus.Text = "label6";
            // 
            // buttonPolacz
            // 
            this.buttonPolacz.Location = new System.Drawing.Point(15, 72);
            this.buttonPolacz.Name = "buttonPolacz";
            this.buttonPolacz.Size = new System.Drawing.Size(125, 23);
            this.buttonPolacz.TabIndex = 10;
            this.buttonPolacz.Text = "Połącz";
            this.buttonPolacz.UseVisualStyleBackColor = true;
            this.buttonPolacz.Click += new System.EventHandler(this.button5_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(57, 110);
            this.textBox1.MaxLength = 64;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(162, 20);
            this.textBox1.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Hasło:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonPolacz);
            this.Controls.Add(this.labelConnectionStatus);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonUstawienia);
            this.Controls.Add(this.labelUserNumber);
            this.Controls.Add(this.labelUserStatus);
            this.Controls.Add(this.buttonLogowanie);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonRejestracja);
            this.Controls.Add(this.buttonRozmowy);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Komunikator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRejestracja;
        private System.Windows.Forms.Button buttonRozmowy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonLogowanie;
        private System.Windows.Forms.Label labelUserStatus;
        private System.Windows.Forms.Label labelUserNumber;
        private System.Windows.Forms.Button buttonUstawienia;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.Button buttonPolacz;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
    }
}

