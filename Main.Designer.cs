using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace osu_taiko_SV_Helper
{
    partial class SvHelper
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvHelper));
            this.Background_Picture_Box = new System.Windows.Forms.PictureBox();
            this.VERSION_LABEL = new System.Windows.Forms.Label();
            this.ARTIST_LABEL = new System.Windows.Forms.Label();
            this.TITLE_LABEL = new System.Windows.Forms.Label();
            this.START_POINT_LABEL = new System.Windows.Forms.Label();
            this.SV_STARTTIME_TEXTBOX = new System.Windows.Forms.TextBox();
            this.SV_ENDTIME_TEXTBOX = new System.Windows.Forms.TextBox();
            this.SV_END_LABEL = new System.Windows.Forms.Label();
            this.SET_START_TIME_BUTTON = new System.Windows.Forms.Button();
            this.SET_END_TIME_BUTTON = new System.Windows.Forms.Button();
            this.SV_START_TEXTBOX = new System.Windows.Forms.TextBox();
            this.SV_END_TEXTBOX = new System.Windows.Forms.TextBox();
            this.VOLUME_END_TEXTBOX = new System.Windows.Forms.TextBox();
            this.VOLUME_START_TEXTBOX = new System.Windows.Forms.TextBox();
            this.OFFSET_TEXTBOX = new System.Windows.Forms.TextBox();
            this.SV_MODE_COMBOBOX = new System.Windows.Forms.ComboBox();
            this.TIME_LABEL = new System.Windows.Forms.Label();
            this.SV_LABEL = new System.Windows.Forms.Label();
            this.VOLUME_LABEL = new System.Windows.Forms.Label();
            this.ARROW_LABEL1 = new System.Windows.Forms.Label();
            this.ARROW_LABEL2 = new System.Windows.Forms.Label();
            this.ARROW_LABEL3 = new System.Windows.Forms.Label();
            this.OFFSET_LABEL = new System.Windows.Forms.Label();
            this.SV_MODE_LABEL = new System.Windows.Forms.Label();
            this.KIAI_MODE_CHECKBOX = new System.Windows.Forms.CheckBox();
            this.MODE_LABEL = new System.Windows.Forms.Label();
            this.MODE_COMBOBOX = new System.Windows.Forms.ComboBox();
            this.WORK_STATUS_LABEL = new System.Windows.Forms.Label();
            this.WORK_STATUS_TEXT = new System.Windows.Forms.Label();
            this.MAKE_BUTTON = new System.Windows.Forms.Button();
            this.RESET_BUTTON = new System.Windows.Forms.Button();
            this.UNDO_BUTTON = new System.Windows.Forms.Button();
            this.OFFSET_CHECKBOX16 = new System.Windows.Forms.CheckBox();
            this.OFFSET_CHECKBOX12 = new System.Windows.Forms.CheckBox();
            this.BPM_COMP_CHECKBOX = new System.Windows.Forms.CheckBox();
            this.BASE_BPM_TEXTBOX = new System.Windows.Forms.TextBox();
            this.BASE_BPM_LABEL = new System.Windows.Forms.Label();
            this.USE_CUSTOM_BPM_CHECKBOX = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Background_Picture_Box)).BeginInit();
            this.Background_Picture_Box.SuspendLayout();
            this.SuspendLayout();
            // 
            // Background_Picture_Box
            // 
            this.Background_Picture_Box.Controls.Add(this.VERSION_LABEL);
            this.Background_Picture_Box.Controls.Add(this.ARTIST_LABEL);
            this.Background_Picture_Box.Controls.Add(this.TITLE_LABEL);
            this.Background_Picture_Box.Location = new System.Drawing.Point(12, 12);
            this.Background_Picture_Box.Name = "Background_Picture_Box";
            this.Background_Picture_Box.Size = new System.Drawing.Size(392, 102);
            this.Background_Picture_Box.TabIndex = 0;
            this.Background_Picture_Box.TabStop = false;
            // 
            // VERSION_LABEL
            // 
            this.VERSION_LABEL.AutoSize = true;
            this.VERSION_LABEL.BackColor = System.Drawing.Color.Transparent;
            this.VERSION_LABEL.Font = new System.Drawing.Font("メイリオ", 14F);
            this.VERSION_LABEL.ForeColor = System.Drawing.Color.White;
            this.VERSION_LABEL.Location = new System.Drawing.Point(290, 9);
            this.VERSION_LABEL.Name = "VERSION_LABEL";
            this.VERSION_LABEL.Size = new System.Drawing.Size(99, 28);
            this.VERSION_LABEL.TabIndex = 32;
            this.VERSION_LABEL.Text = "Unknown";
            // 
            // ARTIST_LABEL
            // 
            this.ARTIST_LABEL.AutoSize = true;
            this.ARTIST_LABEL.BackColor = System.Drawing.Color.Transparent;
            this.ARTIST_LABEL.Font = new System.Drawing.Font("メイリオ", 10F);
            this.ARTIST_LABEL.ForeColor = System.Drawing.Color.White;
            this.ARTIST_LABEL.Location = new System.Drawing.Point(4, 80);
            this.ARTIST_LABEL.Name = "ARTIST_LABEL";
            this.ARTIST_LABEL.Size = new System.Drawing.Size(75, 21);
            this.ARTIST_LABEL.TabIndex = 31;
            this.ARTIST_LABEL.Text = "Unknown";
            // 
            // TITLE_LABEL
            // 
            this.TITLE_LABEL.AutoSize = true;
            this.TITLE_LABEL.BackColor = System.Drawing.Color.Transparent;
            this.TITLE_LABEL.Font = new System.Drawing.Font("メイリオ", 14F);
            this.TITLE_LABEL.ForeColor = System.Drawing.Color.White;
            this.TITLE_LABEL.Location = new System.Drawing.Point(4, 59);
            this.TITLE_LABEL.Name = "TITLE_LABEL";
            this.TITLE_LABEL.Size = new System.Drawing.Size(193, 28);
            this.TITLE_LABEL.TabIndex = 30;
            this.TITLE_LABEL.Text = "osu!taiko SV Helper";
            // 
            // START_POINT_LABEL
            // 
            this.START_POINT_LABEL.AutoSize = true;
            this.START_POINT_LABEL.Font = new System.Drawing.Font("メイリオ", 12F);
            this.START_POINT_LABEL.Location = new System.Drawing.Point(106, 160);
            this.START_POINT_LABEL.Name = "START_POINT_LABEL";
            this.START_POINT_LABEL.Size = new System.Drawing.Size(93, 24);
            this.START_POINT_LABEL.TabIndex = 1;
            this.START_POINT_LABEL.Text = "Start Point";
            // 
            // SV_STARTTIME_TEXTBOX
            // 
            this.SV_STARTTIME_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SV_STARTTIME_TEXTBOX.Enabled = false;
            this.SV_STARTTIME_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.SV_STARTTIME_TEXTBOX.Location = new System.Drawing.Point(110, 188);
            this.SV_STARTTIME_TEXTBOX.Name = "SV_STARTTIME_TEXTBOX";
            this.SV_STARTTIME_TEXTBOX.Size = new System.Drawing.Size(112, 27);
            this.SV_STARTTIME_TEXTBOX.TabIndex = 2;
            this.SV_STARTTIME_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SV_ENDTIME_TEXTBOX
            // 
            this.SV_ENDTIME_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SV_ENDTIME_TEXTBOX.Enabled = false;
            this.SV_ENDTIME_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.SV_ENDTIME_TEXTBOX.Location = new System.Drawing.Point(276, 188);
            this.SV_ENDTIME_TEXTBOX.Name = "SV_ENDTIME_TEXTBOX";
            this.SV_ENDTIME_TEXTBOX.Size = new System.Drawing.Size(112, 27);
            this.SV_ENDTIME_TEXTBOX.TabIndex = 3;
            this.SV_ENDTIME_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SV_END_LABEL
            // 
            this.SV_END_LABEL.AutoSize = true;
            this.SV_END_LABEL.Font = new System.Drawing.Font("メイリオ", 12F);
            this.SV_END_LABEL.Location = new System.Drawing.Point(272, 161);
            this.SV_END_LABEL.Name = "SV_END_LABEL";
            this.SV_END_LABEL.Size = new System.Drawing.Size(85, 24);
            this.SV_END_LABEL.TabIndex = 4;
            this.SV_END_LABEL.Text = "End Point";
            // 
            // SET_START_TIME_BUTTON
            // 
            this.SET_START_TIME_BUTTON.Enabled = false;
            this.SET_START_TIME_BUTTON.Font = new System.Drawing.Font("メイリオ", 9F);
            this.SET_START_TIME_BUTTON.Location = new System.Drawing.Point(110, 221);
            this.SET_START_TIME_BUTTON.Name = "SET_START_TIME_BUTTON";
            this.SET_START_TIME_BUTTON.Size = new System.Drawing.Size(112, 23);
            this.SET_START_TIME_BUTTON.TabIndex = 5;
            this.SET_START_TIME_BUTTON.Text = "Set start time";
            this.SET_START_TIME_BUTTON.UseVisualStyleBackColor = true;
            this.SET_START_TIME_BUTTON.Click += new System.EventHandler(this.SET_START_TIME_BUTTON_Click);
            // 
            // SET_END_TIME_BUTTON
            // 
            this.SET_END_TIME_BUTTON.Enabled = false;
            this.SET_END_TIME_BUTTON.Font = new System.Drawing.Font("メイリオ", 9F);
            this.SET_END_TIME_BUTTON.Location = new System.Drawing.Point(276, 222);
            this.SET_END_TIME_BUTTON.Name = "SET_END_TIME_BUTTON";
            this.SET_END_TIME_BUTTON.Size = new System.Drawing.Size(112, 23);
            this.SET_END_TIME_BUTTON.TabIndex = 6;
            this.SET_END_TIME_BUTTON.Text = "Set end time";
            this.SET_END_TIME_BUTTON.UseVisualStyleBackColor = true;
            this.SET_END_TIME_BUTTON.Click += new System.EventHandler(this.SET_END_TIME_BUTTON_Click);
            // 
            // SV_START_TEXTBOX
            // 
            this.SV_START_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SV_START_TEXTBOX.Enabled = false;
            this.SV_START_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.SV_START_TEXTBOX.Location = new System.Drawing.Point(110, 261);
            this.SV_START_TEXTBOX.Name = "SV_START_TEXTBOX";
            this.SV_START_TEXTBOX.Size = new System.Drawing.Size(112, 27);
            this.SV_START_TEXTBOX.TabIndex = 7;
            this.SV_START_TEXTBOX.Text = "1";
            this.SV_START_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SV_END_TEXTBOX
            // 
            this.SV_END_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SV_END_TEXTBOX.Enabled = false;
            this.SV_END_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.SV_END_TEXTBOX.Location = new System.Drawing.Point(276, 262);
            this.SV_END_TEXTBOX.Name = "SV_END_TEXTBOX";
            this.SV_END_TEXTBOX.Size = new System.Drawing.Size(112, 27);
            this.SV_END_TEXTBOX.TabIndex = 8;
            this.SV_END_TEXTBOX.Text = "1";
            this.SV_END_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // VOLUME_END_TEXTBOX
            // 
            this.VOLUME_END_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VOLUME_END_TEXTBOX.Enabled = false;
            this.VOLUME_END_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.VOLUME_END_TEXTBOX.Location = new System.Drawing.Point(276, 305);
            this.VOLUME_END_TEXTBOX.Name = "VOLUME_END_TEXTBOX";
            this.VOLUME_END_TEXTBOX.Size = new System.Drawing.Size(112, 27);
            this.VOLUME_END_TEXTBOX.TabIndex = 10;
            this.VOLUME_END_TEXTBOX.Text = "100";
            this.VOLUME_END_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // VOLUME_START_TEXTBOX
            // 
            this.VOLUME_START_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VOLUME_START_TEXTBOX.Enabled = false;
            this.VOLUME_START_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.VOLUME_START_TEXTBOX.Location = new System.Drawing.Point(110, 304);
            this.VOLUME_START_TEXTBOX.Name = "VOLUME_START_TEXTBOX";
            this.VOLUME_START_TEXTBOX.Size = new System.Drawing.Size(112, 27);
            this.VOLUME_START_TEXTBOX.TabIndex = 9;
            this.VOLUME_START_TEXTBOX.Text = "100";
            this.VOLUME_START_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // OFFSET_TEXTBOX
            // 
            this.OFFSET_TEXTBOX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OFFSET_TEXTBOX.Enabled = false;
            this.OFFSET_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.OFFSET_TEXTBOX.Location = new System.Drawing.Point(110, 347);
            this.OFFSET_TEXTBOX.Name = "OFFSET_TEXTBOX";
            this.OFFSET_TEXTBOX.Size = new System.Drawing.Size(112, 27);
            this.OFFSET_TEXTBOX.TabIndex = 11;
            this.OFFSET_TEXTBOX.Text = "0";
            this.OFFSET_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SV_MODE_COMBOBOX
            // 
            this.SV_MODE_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SV_MODE_COMBOBOX.Enabled = false;
            this.SV_MODE_COMBOBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.SV_MODE_COMBOBOX.FormattingEnabled = true;
            this.SV_MODE_COMBOBOX.Items.AddRange(new object[] {
            "Arithmetic Mode(等差数列SV / デフォルト)",
            "Geometric Mode(等比数列SV)",
            "Dance Mode(等差数列1/16 SV)"});
            this.SV_MODE_COMBOBOX.Location = new System.Drawing.Point(110, 382);
            this.SV_MODE_COMBOBOX.Name = "SV_MODE_COMBOBOX";
            this.SV_MODE_COMBOBOX.Size = new System.Drawing.Size(278, 28);
            this.SV_MODE_COMBOBOX.TabIndex = 12;
            // 
            // TIME_LABEL
            // 
            this.TIME_LABEL.AutoSize = true;
            this.TIME_LABEL.Font = new System.Drawing.Font("メイリオ", 13F);
            this.TIME_LABEL.Location = new System.Drawing.Point(41, 187);
            this.TIME_LABEL.Name = "TIME_LABEL";
            this.TIME_LABEL.Size = new System.Drawing.Size(63, 27);
            this.TIME_LABEL.TabIndex = 13;
            this.TIME_LABEL.Text = "Time:";
            // 
            // SV_LABEL
            // 
            this.SV_LABEL.AutoSize = true;
            this.SV_LABEL.Font = new System.Drawing.Font("メイリオ", 13F);
            this.SV_LABEL.Location = new System.Drawing.Point(62, 260);
            this.SV_LABEL.Name = "SV_LABEL";
            this.SV_LABEL.Size = new System.Drawing.Size(42, 27);
            this.SV_LABEL.TabIndex = 14;
            this.SV_LABEL.Text = "SV:";
            // 
            // VOLUME_LABEL
            // 
            this.VOLUME_LABEL.AutoSize = true;
            this.VOLUME_LABEL.Font = new System.Drawing.Font("メイリオ", 13F);
            this.VOLUME_LABEL.Location = new System.Drawing.Point(19, 303);
            this.VOLUME_LABEL.Name = "VOLUME_LABEL";
            this.VOLUME_LABEL.Size = new System.Drawing.Size(85, 27);
            this.VOLUME_LABEL.TabIndex = 15;
            this.VOLUME_LABEL.Text = "Volume:";
            // 
            // ARROW_LABEL1
            // 
            this.ARROW_LABEL1.AutoSize = true;
            this.ARROW_LABEL1.Font = new System.Drawing.Font("メイリオ", 13F);
            this.ARROW_LABEL1.Location = new System.Drawing.Point(236, 189);
            this.ARROW_LABEL1.Name = "ARROW_LABEL1";
            this.ARROW_LABEL1.Size = new System.Drawing.Size(30, 27);
            this.ARROW_LABEL1.TabIndex = 16;
            this.ARROW_LABEL1.Text = "→";
            this.ARROW_LABEL1.Click += new System.EventHandler(this.ARROW_LABEL1_Click);
            // 
            // ARROW_LABEL2
            // 
            this.ARROW_LABEL2.AutoSize = true;
            this.ARROW_LABEL2.Font = new System.Drawing.Font("メイリオ", 13F);
            this.ARROW_LABEL2.Location = new System.Drawing.Point(236, 264);
            this.ARROW_LABEL2.Name = "ARROW_LABEL2";
            this.ARROW_LABEL2.Size = new System.Drawing.Size(30, 27);
            this.ARROW_LABEL2.TabIndex = 17;
            this.ARROW_LABEL2.Text = "→";
            this.ARROW_LABEL2.Click += new System.EventHandler(this.ARROW_LABEL2_Click);
            // 
            // ARROW_LABEL3
            // 
            this.ARROW_LABEL3.AutoSize = true;
            this.ARROW_LABEL3.Font = new System.Drawing.Font("メイリオ", 13F);
            this.ARROW_LABEL3.Location = new System.Drawing.Point(236, 307);
            this.ARROW_LABEL3.Name = "ARROW_LABEL3";
            this.ARROW_LABEL3.Size = new System.Drawing.Size(30, 27);
            this.ARROW_LABEL3.TabIndex = 18;
            this.ARROW_LABEL3.Text = "→";
            this.ARROW_LABEL3.Click += new System.EventHandler(this.ARROW_LABEL3_Click);
            // 
            // OFFSET_LABEL
            // 
            this.OFFSET_LABEL.AutoSize = true;
            this.OFFSET_LABEL.Font = new System.Drawing.Font("メイリオ", 13F);
            this.OFFSET_LABEL.Location = new System.Drawing.Point(32, 345);
            this.OFFSET_LABEL.Name = "OFFSET_LABEL";
            this.OFFSET_LABEL.Size = new System.Drawing.Size(72, 27);
            this.OFFSET_LABEL.TabIndex = 19;
            this.OFFSET_LABEL.Text = "Offset:";
            // 
            // SV_MODE_LABEL
            // 
            this.SV_MODE_LABEL.AutoSize = true;
            this.SV_MODE_LABEL.Font = new System.Drawing.Font("メイリオ", 13F);
            this.SV_MODE_LABEL.Location = new System.Drawing.Point(8, 383);
            this.SV_MODE_LABEL.Name = "SV_MODE_LABEL";
            this.SV_MODE_LABEL.Size = new System.Drawing.Size(96, 27);
            this.SV_MODE_LABEL.TabIndex = 21;
            this.SV_MODE_LABEL.Text = "SV Mode:";
            // 
            // KIAI_MODE_CHECKBOX
            // 
            this.KIAI_MODE_CHECKBOX.AutoSize = true;
            this.KIAI_MODE_CHECKBOX.Enabled = false;
            this.KIAI_MODE_CHECKBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.KIAI_MODE_CHECKBOX.Location = new System.Drawing.Point(25, 456);
            this.KIAI_MODE_CHECKBOX.Name = "KIAI_MODE_CHECKBOX";
            this.KIAI_MODE_CHECKBOX.Size = new System.Drawing.Size(97, 25);
            this.KIAI_MODE_CHECKBOX.TabIndex = 22;
            this.KIAI_MODE_CHECKBOX.Text = "Kiai mode";
            this.KIAI_MODE_CHECKBOX.UseVisualStyleBackColor = true;
            // 
            // MODE_LABEL
            // 
            this.MODE_LABEL.AutoSize = true;
            this.MODE_LABEL.Font = new System.Drawing.Font("メイリオ", 13F);
            this.MODE_LABEL.Location = new System.Drawing.Point(37, 423);
            this.MODE_LABEL.Name = "MODE_LABEL";
            this.MODE_LABEL.Size = new System.Drawing.Size(67, 27);
            this.MODE_LABEL.TabIndex = 23;
            this.MODE_LABEL.Text = "Mode:";
            // 
            // MODE_COMBOBOX
            // 
            this.MODE_COMBOBOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MODE_COMBOBOX.Enabled = false;
            this.MODE_COMBOBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.MODE_COMBOBOX.FormattingEnabled = true;
            this.MODE_COMBOBOX.Items.AddRange(new object[] {
            "Overwrite(上書き)",
            "Append(追加)",
            "Preserve(前のデータを優先)",
            "Multiply(重ねがけ)"});
            this.MODE_COMBOBOX.Location = new System.Drawing.Point(110, 422);
            this.MODE_COMBOBOX.Name = "MODE_COMBOBOX";
            this.MODE_COMBOBOX.Size = new System.Drawing.Size(278, 28);
            this.MODE_COMBOBOX.TabIndex = 24;
            // 
            // WORK_STATUS_LABEL
            // 
            this.WORK_STATUS_LABEL.AutoSize = true;
            this.WORK_STATUS_LABEL.Font = new System.Drawing.Font("メイリオ", 12F);
            this.WORK_STATUS_LABEL.Location = new System.Drawing.Point(12, 126);
            this.WORK_STATUS_LABEL.Name = "WORK_STATUS_LABEL";
            this.WORK_STATUS_LABEL.Size = new System.Drawing.Size(110, 24);
            this.WORK_STATUS_LABEL.TabIndex = 25;
            this.WORK_STATUS_LABEL.Text = "Work status:";
            // 
            // WORK_STATUS_TEXT
            // 
            this.WORK_STATUS_TEXT.AutoSize = true;
            this.WORK_STATUS_TEXT.Font = new System.Drawing.Font("メイリオ", 12F);
            this.WORK_STATUS_TEXT.ForeColor = System.Drawing.Color.Green;
            this.WORK_STATUS_TEXT.Location = new System.Drawing.Point(119, 126);
            this.WORK_STATUS_TEXT.Name = "WORK_STATUS_TEXT";
            this.WORK_STATUS_TEXT.Size = new System.Drawing.Size(179, 24);
            this.WORK_STATUS_TEXT.TabIndex = 26;
            this.WORK_STATUS_TEXT.Text = "Launching... just wait";
            // 
            // MAKE_BUTTON
            // 
            this.MAKE_BUTTON.Enabled = false;
            this.MAKE_BUTTON.Font = new System.Drawing.Font("メイリオ", 12F);
            this.MAKE_BUTTON.Location = new System.Drawing.Point(272, 532);
            this.MAKE_BUTTON.Name = "MAKE_BUTTON";
            this.MAKE_BUTTON.Size = new System.Drawing.Size(125, 48);
            this.MAKE_BUTTON.TabIndex = 27;
            this.MAKE_BUTTON.Text = "Make";
            this.MAKE_BUTTON.UseVisualStyleBackColor = true;
            this.MAKE_BUTTON.Click += new System.EventHandler(this.MAKE_BUTTON_Click);
            // 
            // RESET_BUTTON
            // 
            this.RESET_BUTTON.Enabled = false;
            this.RESET_BUTTON.Font = new System.Drawing.Font("メイリオ", 12F);
            this.RESET_BUTTON.Location = new System.Drawing.Point(141, 532);
            this.RESET_BUTTON.Name = "RESET_BUTTON";
            this.RESET_BUTTON.Size = new System.Drawing.Size(125, 48);
            this.RESET_BUTTON.TabIndex = 28;
            this.RESET_BUTTON.Text = "Reset";
            this.RESET_BUTTON.UseVisualStyleBackColor = true;
            this.RESET_BUTTON.Click += new System.EventHandler(this.RESET_BUTTON_Click);
            // 
            // UNDO_BUTTON
            // 
            this.UNDO_BUTTON.Enabled = false;
            this.UNDO_BUTTON.Font = new System.Drawing.Font("メイリオ", 12F);
            this.UNDO_BUTTON.Location = new System.Drawing.Point(10, 532);
            this.UNDO_BUTTON.Name = "UNDO_BUTTON";
            this.UNDO_BUTTON.Size = new System.Drawing.Size(125, 48);
            this.UNDO_BUTTON.TabIndex = 29;
            this.UNDO_BUTTON.Text = "Undo";
            this.UNDO_BUTTON.UseVisualStyleBackColor = true;
            this.UNDO_BUTTON.Click += new System.EventHandler(this.UNDO_BUTTON_Click);
            // 
            // OFFSET_CHECKBOX16
            // 
            this.OFFSET_CHECKBOX16.AutoSize = true;
            this.OFFSET_CHECKBOX16.Enabled = false;
            this.OFFSET_CHECKBOX16.Font = new System.Drawing.Font("メイリオ", 10F);
            this.OFFSET_CHECKBOX16.Location = new System.Drawing.Point(141, 456);
            this.OFFSET_CHECKBOX16.Name = "OFFSET_CHECKBOX16";
            this.OFFSET_CHECKBOX16.Size = new System.Drawing.Size(113, 25);
            this.OFFSET_CHECKBOX16.TabIndex = 31;
            this.OFFSET_CHECKBOX16.Text = "-1/16 Offset";
            this.OFFSET_CHECKBOX16.UseVisualStyleBackColor = true;
            this.OFFSET_CHECKBOX16.CheckedChanged += new System.EventHandler(this.OFFSET_CHECKBOX16_CheckedChanged);
            // 
            // OFFSET_CHECKBOX12
            // 
            this.OFFSET_CHECKBOX12.AutoSize = true;
            this.OFFSET_CHECKBOX12.Enabled = false;
            this.OFFSET_CHECKBOX12.Font = new System.Drawing.Font("メイリオ", 10F);
            this.OFFSET_CHECKBOX12.Location = new System.Drawing.Point(272, 456);
            this.OFFSET_CHECKBOX12.Name = "OFFSET_CHECKBOX12";
            this.OFFSET_CHECKBOX12.Size = new System.Drawing.Size(113, 25);
            this.OFFSET_CHECKBOX12.TabIndex = 32;
            this.OFFSET_CHECKBOX12.Text = "-1/12 Offset";
            this.OFFSET_CHECKBOX12.UseVisualStyleBackColor = true;
            this.OFFSET_CHECKBOX12.CheckedChanged += new System.EventHandler(this.OFFSET_CHECKBOX12_CheckedChanged);
            // 
            // BPM_COMP_CHECKBOX
            // 
            this.BPM_COMP_CHECKBOX.AutoSize = true;
            this.BPM_COMP_CHECKBOX.Enabled = false;
            this.BPM_COMP_CHECKBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.BPM_COMP_CHECKBOX.Location = new System.Drawing.Point(24, 477);
            this.BPM_COMP_CHECKBOX.Name = "BPM_COMP_CHECKBOX";
            this.BPM_COMP_CHECKBOX.Size = new System.Drawing.Size(153, 25);
            this.BPM_COMP_CHECKBOX.TabIndex = 33;
            this.BPM_COMP_CHECKBOX.Text = "BPM Compatibility";
            this.BPM_COMP_CHECKBOX.UseVisualStyleBackColor = true;
            this.BPM_COMP_CHECKBOX.CheckedChanged += new System.EventHandler(this.BPM_COMP_CHECKBOX_CheckedChanged);
            // 
            // BASE_BPM_TEXTBOX
            // 
            this.BASE_BPM_TEXTBOX.Enabled = false;
            this.BASE_BPM_TEXTBOX.Font = new System.Drawing.Font("メイリオ", 9F);
            this.BASE_BPM_TEXTBOX.Location = new System.Drawing.Point(285, 478);
            this.BASE_BPM_TEXTBOX.Name = "BASE_BPM_TEXTBOX";
            this.BASE_BPM_TEXTBOX.Size = new System.Drawing.Size(100, 25);
            this.BASE_BPM_TEXTBOX.TabIndex = 34;
            this.BASE_BPM_TEXTBOX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.BASE_BPM_TEXTBOX.Visible = false;
            // 
            // BASE_BPM_LABEL
            // 
            this.BASE_BPM_LABEL.AutoSize = true;
            this.BASE_BPM_LABEL.Enabled = false;
            this.BASE_BPM_LABEL.Font = new System.Drawing.Font("メイリオ", 10F);
            this.BASE_BPM_LABEL.Location = new System.Drawing.Point(199, 480);
            this.BASE_BPM_LABEL.Name = "BASE_BPM_LABEL";
            this.BASE_BPM_LABEL.Size = new System.Drawing.Size(82, 21);
            this.BASE_BPM_LABEL.TabIndex = 35;
            this.BASE_BPM_LABEL.Text = "Base BPM:";
            this.BASE_BPM_LABEL.Visible = false;
            // 
            // USE_CUSTOM_BPM_CHECKBOX
            // 
            this.USE_CUSTOM_BPM_CHECKBOX.AutoSize = true;
            this.USE_CUSTOM_BPM_CHECKBOX.Enabled = false;
            this.USE_CUSTOM_BPM_CHECKBOX.Font = new System.Drawing.Font("メイリオ", 10F);
            this.USE_CUSTOM_BPM_CHECKBOX.Location = new System.Drawing.Point(24, 498);
            this.USE_CUSTOM_BPM_CHECKBOX.Name = "USE_CUSTOM_BPM_CHECKBOX";
            this.USE_CUSTOM_BPM_CHECKBOX.Size = new System.Drawing.Size(181, 25);
            this.USE_CUSTOM_BPM_CHECKBOX.TabIndex = 36;
            this.USE_CUSTOM_BPM_CHECKBOX.Text = "Use Custom Base BPM";
            this.USE_CUSTOM_BPM_CHECKBOX.UseVisualStyleBackColor = true;
            this.USE_CUSTOM_BPM_CHECKBOX.Visible = false;
            this.USE_CUSTOM_BPM_CHECKBOX.CheckedChanged += new System.EventHandler(this.USE_CUSTOM_BPM_CHECKBOX_CheckedChanged);
            // 
            // SvHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(416, 592);
            this.Controls.Add(this.USE_CUSTOM_BPM_CHECKBOX);
            this.Controls.Add(this.BASE_BPM_LABEL);
            this.Controls.Add(this.BASE_BPM_TEXTBOX);
            this.Controls.Add(this.BPM_COMP_CHECKBOX);
            this.Controls.Add(this.OFFSET_CHECKBOX12);
            this.Controls.Add(this.OFFSET_CHECKBOX16);
            this.Controls.Add(this.UNDO_BUTTON);
            this.Controls.Add(this.RESET_BUTTON);
            this.Controls.Add(this.MAKE_BUTTON);
            this.Controls.Add(this.WORK_STATUS_TEXT);
            this.Controls.Add(this.WORK_STATUS_LABEL);
            this.Controls.Add(this.MODE_COMBOBOX);
            this.Controls.Add(this.MODE_LABEL);
            this.Controls.Add(this.KIAI_MODE_CHECKBOX);
            this.Controls.Add(this.SV_MODE_LABEL);
            this.Controls.Add(this.OFFSET_LABEL);
            this.Controls.Add(this.ARROW_LABEL3);
            this.Controls.Add(this.ARROW_LABEL2);
            this.Controls.Add(this.ARROW_LABEL1);
            this.Controls.Add(this.VOLUME_LABEL);
            this.Controls.Add(this.SV_LABEL);
            this.Controls.Add(this.TIME_LABEL);
            this.Controls.Add(this.SV_MODE_COMBOBOX);
            this.Controls.Add(this.OFFSET_TEXTBOX);
            this.Controls.Add(this.VOLUME_END_TEXTBOX);
            this.Controls.Add(this.VOLUME_START_TEXTBOX);
            this.Controls.Add(this.SV_END_TEXTBOX);
            this.Controls.Add(this.SV_START_TEXTBOX);
            this.Controls.Add(this.SET_END_TIME_BUTTON);
            this.Controls.Add(this.SET_START_TIME_BUTTON);
            this.Controls.Add(this.SV_END_LABEL);
            this.Controls.Add(this.SV_ENDTIME_TEXTBOX);
            this.Controls.Add(this.SV_STARTTIME_TEXTBOX);
            this.Controls.Add(this.START_POINT_LABEL);
            this.Controls.Add(this.Background_Picture_Box);
            this.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SvHelper";
            this.Text = "osu!taiko SV Helper";
            ((System.ComponentModel.ISupportInitialize)(this.Background_Picture_Box)).EndInit();
            this.Background_Picture_Box.ResumeLayout(false);
            this.Background_Picture_Box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.CheckBox OFFSET_CHECKBOX16;
        private System.Windows.Forms.CheckBox OFFSET_CHECKBOX12;

        #endregion

        private System.Windows.Forms.PictureBox Background_Picture_Box;
        private Label START_POINT_LABEL;
        private System.Windows.Forms.TextBox SV_STARTTIME_TEXTBOX;
        private System.Windows.Forms.TextBox SV_ENDTIME_TEXTBOX;
        private Label SV_END_LABEL;
        private System.Windows.Forms.Button SET_START_TIME_BUTTON;
        private System.Windows.Forms.Button SET_END_TIME_BUTTON;
        private System.Windows.Forms.TextBox SV_START_TEXTBOX;
        private System.Windows.Forms.TextBox SV_END_TEXTBOX;
        private System.Windows.Forms.TextBox VOLUME_END_TEXTBOX;
        private System.Windows.Forms.TextBox VOLUME_START_TEXTBOX;
        private System.Windows.Forms.TextBox OFFSET_TEXTBOX;
        private System.Windows.Forms.ComboBox SV_MODE_COMBOBOX;
        private Label TIME_LABEL;
        private Label SV_LABEL;
        private Label VOLUME_LABEL;
        private Label ARROW_LABEL1;
        private Label ARROW_LABEL2;
        private Label ARROW_LABEL3;
        private Label OFFSET_LABEL;
        private Label SV_MODE_LABEL;
        private System.Windows.Forms.CheckBox KIAI_MODE_CHECKBOX;
        private Label MODE_LABEL;
        private System.Windows.Forms.ComboBox MODE_COMBOBOX;
        private System.Windows.Forms.Label WORK_STATUS_LABEL;
        private System.Windows.Forms.Label WORK_STATUS_TEXT;
        private System.Windows.Forms.Button MAKE_BUTTON;
        private System.Windows.Forms.Button RESET_BUTTON;
        private System.Windows.Forms.Button UNDO_BUTTON;
        private System.Windows.Forms.Label TITLE_LABEL;
        private System.Windows.Forms.Label ARTIST_LABEL;
        private System.Windows.Forms.Label VERSION_LABEL;
        private CheckBox BPM_COMP_CHECKBOX;
        private TextBox BASE_BPM_TEXTBOX;
        private Label BASE_BPM_LABEL;
        private CheckBox USE_CUSTOM_BPM_CHECKBOX;
    }
}

