using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ParameterManager;

namespace KPVisionInspectionFramework
{
    public partial class DummyCountWindow : Form
    {
        public delegate void SetDummyCountHandler(uint _OriginDummyCount);
        public event SetDummyCountHandler SetDummyCountEvent;

        public DummyCountWindow()
        {
            InitializeComponent();

            numUpDownDummyCount.Text = "";

            this.ActiveControl = numUpDownDummyCount;
            numUpDownDummyCount.Focus();
        }

        public void ShowWindow()
        {
            this.ShowDialog();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (numUpDownDummyCount.Text == "") { MessageBox.Show("Dummy 수량을 입력하세요."); return; }
            else
            {
                DialogResult _MsgboxResult;
                _MsgboxResult = MessageBox.Show("Dummy를 " + numUpDownDummyCount.Text + " 개로 설정 하시겠습니까?", "Check", MessageBoxButtons.YesNo);

                if (_MsgboxResult == DialogResult.Yes)
                {
                    SetDummyCountEvent(Convert.ToUInt32(numUpDownDummyCount.Text));

                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                }
                else return;
            }
        }

        #region Control Event
        private void labelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            var s = sender as Label;
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            s.Parent.Left = this.Left + (e.X - ((Point)s.Tag).X);
            s.Parent.Top = this.Top + (e.Y - ((Point)s.Tag).Y);

            this.Cursor = Cursors.Default;
        }

        private void labelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            var s = sender as Label;
            s.Tag = new Point(e.X, e.Y);
        }

        private void textBoxNewRecipe_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                e.Handled = true;
            }
        }
        #endregion Control Event
    }
}
