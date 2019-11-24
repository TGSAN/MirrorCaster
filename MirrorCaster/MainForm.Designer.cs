namespace MirrorCaster
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.startCastButton = new System.Windows.Forms.Button();
            this.stopCastButton = new System.Windows.Forms.Button();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.screenBox = new System.Windows.Forms.Panel();
            this.nosigalLabel = new System.Windows.Forms.Label();
            this.heartTimer = new System.Windows.Forms.Timer(this.components);
            this.powerKeyButton = new System.Windows.Forms.Button();
            this.backKeyButton = new System.Windows.Forms.Button();
            this.homeKeyButton = new System.Windows.Forms.Button();
            this.controlPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.testButton = new System.Windows.Forms.Button();
            this.mutiKeyButton = new System.Windows.Forms.Button();
            this.menuKeyButton = new System.Windows.Forms.Button();
            this.volUpKeyButton = new System.Windows.Forms.Button();
            this.volDownKeyButton = new System.Windows.Forms.Button();
            this.startCastSingleButton = new System.Windows.Forms.Button();
            this.screenBox.SuspendLayout();
            this.controlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // startCastButton
            // 
            this.startCastButton.Location = new System.Drawing.Point(3, 4);
            this.startCastButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.startCastButton.Name = "startCastButton";
            this.startCastButton.Size = new System.Drawing.Size(87, 33);
            this.startCastButton.TabIndex = 0;
            this.startCastButton.Text = "开始投屏";
            this.startCastButton.UseVisualStyleBackColor = true;
            this.startCastButton.Click += new System.EventHandler(this.StartCastButton_Click);
            // 
            // stopCastButton
            // 
            this.stopCastButton.Enabled = false;
            this.stopCastButton.Location = new System.Drawing.Point(459, 4);
            this.stopCastButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.stopCastButton.Name = "stopCastButton";
            this.stopCastButton.Size = new System.Drawing.Size(87, 33);
            this.stopCastButton.TabIndex = 1;
            this.stopCastButton.Text = "结束投屏";
            this.stopCastButton.UseVisualStyleBackColor = true;
            this.stopCastButton.Click += new System.EventHandler(this.StopCastButton_Click);
            // 
            // screenBox
            // 
            this.screenBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.screenBox.BackColor = System.Drawing.Color.Black;
            this.screenBox.Controls.Add(this.nosigalLabel);
            this.screenBox.Location = new System.Drawing.Point(12, 76);
            this.screenBox.Name = "screenBox";
            this.screenBox.Size = new System.Drawing.Size(737, 484);
            this.screenBox.TabIndex = 2;
            // 
            // nosigalLabel
            // 
            this.nosigalLabel.BackColor = System.Drawing.Color.Transparent;
            this.nosigalLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nosigalLabel.Font = new System.Drawing.Font("微软雅黑", 24F);
            this.nosigalLabel.ForeColor = System.Drawing.Color.White;
            this.nosigalLabel.Location = new System.Drawing.Point(0, 0);
            this.nosigalLabel.Name = "nosigalLabel";
            this.nosigalLabel.Size = new System.Drawing.Size(737, 484);
            this.nosigalLabel.TabIndex = 0;
            this.nosigalLabel.Text = "无信号_(:3」∠)_";
            this.nosigalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // heartTimer
            // 
            this.heartTimer.Interval = 1000;
            this.heartTimer.Tick += new System.EventHandler(this.HeartTimer_Tick);
            // 
            // powerKeyButton
            // 
            this.powerKeyButton.Enabled = false;
            this.powerKeyButton.Location = new System.Drawing.Point(552, 4);
            this.powerKeyButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.powerKeyButton.Name = "powerKeyButton";
            this.powerKeyButton.Size = new System.Drawing.Size(87, 33);
            this.powerKeyButton.TabIndex = 4;
            this.powerKeyButton.Text = "电源键";
            this.powerKeyButton.UseVisualStyleBackColor = true;
            this.powerKeyButton.Click += new System.EventHandler(this.PowerKeyButton_Click);
            // 
            // backKeyButton
            // 
            this.backKeyButton.Enabled = false;
            this.backKeyButton.Location = new System.Drawing.Point(645, 4);
            this.backKeyButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.backKeyButton.Name = "backKeyButton";
            this.backKeyButton.Size = new System.Drawing.Size(87, 33);
            this.backKeyButton.TabIndex = 5;
            this.backKeyButton.Text = "返回";
            this.backKeyButton.UseVisualStyleBackColor = true;
            this.backKeyButton.Click += new System.EventHandler(this.BackKeyButton_Click);
            // 
            // homeKeyButton
            // 
            this.homeKeyButton.Enabled = false;
            this.homeKeyButton.Location = new System.Drawing.Point(738, 4);
            this.homeKeyButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.homeKeyButton.Name = "homeKeyButton";
            this.homeKeyButton.Size = new System.Drawing.Size(87, 33);
            this.homeKeyButton.TabIndex = 6;
            this.homeKeyButton.Text = "主屏幕";
            this.homeKeyButton.UseVisualStyleBackColor = true;
            this.homeKeyButton.Click += new System.EventHandler(this.HomeKeyButton_Click);
            // 
            // controlPanel
            // 
            this.controlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controlPanel.AutoScroll = true;
            this.controlPanel.Controls.Add(this.startCastButton);
            this.controlPanel.Controls.Add(this.startCastSingleButton);
            this.controlPanel.Controls.Add(this.testButton);
            this.controlPanel.Controls.Add(this.stopCastButton);
            this.controlPanel.Controls.Add(this.powerKeyButton);
            this.controlPanel.Controls.Add(this.backKeyButton);
            this.controlPanel.Controls.Add(this.homeKeyButton);
            this.controlPanel.Controls.Add(this.mutiKeyButton);
            this.controlPanel.Controls.Add(this.menuKeyButton);
            this.controlPanel.Controls.Add(this.volUpKeyButton);
            this.controlPanel.Controls.Add(this.volDownKeyButton);
            this.controlPanel.Location = new System.Drawing.Point(12, 12);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(737, 58);
            this.controlPanel.TabIndex = 6;
            this.controlPanel.WrapContents = false;
            // 
            // testButton
            // 
            this.testButton.Location = new System.Drawing.Point(366, 4);
            this.testButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(87, 33);
            this.testButton.TabIndex = 15;
            this.testButton.Text = "测试按钮";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Visible = false;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // mutiKeyButton
            // 
            this.mutiKeyButton.Enabled = false;
            this.mutiKeyButton.Location = new System.Drawing.Point(831, 4);
            this.mutiKeyButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mutiKeyButton.Name = "mutiKeyButton";
            this.mutiKeyButton.Size = new System.Drawing.Size(87, 33);
            this.mutiKeyButton.TabIndex = 11;
            this.mutiKeyButton.Text = "多任务";
            this.mutiKeyButton.UseVisualStyleBackColor = true;
            this.mutiKeyButton.Click += new System.EventHandler(this.MutiKeyButton_Click);
            // 
            // menuKeyButton
            // 
            this.menuKeyButton.Enabled = false;
            this.menuKeyButton.Location = new System.Drawing.Point(924, 4);
            this.menuKeyButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.menuKeyButton.Name = "menuKeyButton";
            this.menuKeyButton.Size = new System.Drawing.Size(87, 33);
            this.menuKeyButton.TabIndex = 14;
            this.menuKeyButton.Text = "菜单";
            this.menuKeyButton.UseVisualStyleBackColor = true;
            this.menuKeyButton.Click += new System.EventHandler(this.MenuKeyButton_Click);
            // 
            // volUpKeyButton
            // 
            this.volUpKeyButton.Enabled = false;
            this.volUpKeyButton.Location = new System.Drawing.Point(1017, 4);
            this.volUpKeyButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.volUpKeyButton.Name = "volUpKeyButton";
            this.volUpKeyButton.Size = new System.Drawing.Size(87, 33);
            this.volUpKeyButton.TabIndex = 12;
            this.volUpKeyButton.Text = "音量+";
            this.volUpKeyButton.UseVisualStyleBackColor = true;
            this.volUpKeyButton.Click += new System.EventHandler(this.VolUpKeyButton_Click);
            // 
            // volDownKeyButton
            // 
            this.volDownKeyButton.Enabled = false;
            this.volDownKeyButton.Location = new System.Drawing.Point(1110, 4);
            this.volDownKeyButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.volDownKeyButton.Name = "volDownKeyButton";
            this.volDownKeyButton.Size = new System.Drawing.Size(87, 33);
            this.volDownKeyButton.TabIndex = 13;
            this.volDownKeyButton.Text = "音量-";
            this.volDownKeyButton.UseVisualStyleBackColor = true;
            this.volDownKeyButton.Click += new System.EventHandler(this.VolDownKeyButton_Click);
            // 
            // startCastSingleButton
            // 
            this.startCastSingleButton.Location = new System.Drawing.Point(96, 4);
            this.startCastSingleButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.startCastSingleButton.Name = "startCastSingleButton";
            this.startCastSingleButton.Size = new System.Drawing.Size(264, 33);
            this.startCastSingleButton.TabIndex = 16;
            this.startCastSingleButton.Tag = "Single";
            this.startCastSingleButton.Text = "开始独立窗口投屏 (可用于OBS游戏源捕获)";
            this.startCastSingleButton.UseVisualStyleBackColor = true;
            this.startCastSingleButton.Click += new System.EventHandler(this.startCastSingleButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 572);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.screenBox);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "CloudMoe Mirror Caster";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.screenBox.ResumeLayout(false);
            this.controlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button startCastButton;
        private System.Windows.Forms.Button stopCastButton;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Windows.Forms.Panel screenBox;
        private System.Windows.Forms.Label nosigalLabel;
        private System.Windows.Forms.Timer heartTimer;
        private System.Windows.Forms.Button powerKeyButton;
        private System.Windows.Forms.Button homeKeyButton;
        private System.Windows.Forms.Button backKeyButton;
        private System.Windows.Forms.FlowLayoutPanel controlPanel;
        private System.Windows.Forms.Button mutiKeyButton;
        private System.Windows.Forms.Button volUpKeyButton;
        private System.Windows.Forms.Button volDownKeyButton;
        private System.Windows.Forms.Button menuKeyButton;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.Button startCastSingleButton;
    }
}

