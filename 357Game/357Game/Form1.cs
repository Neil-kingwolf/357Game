using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _357Game
{
    public partial class Form1 : Form
    {
        private delegate void setTxtContent(string msg);
        Dictionary<int, List<int>> dicAll;
        Random ran;
        object obj = new object();
        int step = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dicAll = new Dictionary<int, List<int>>();
            ran = new Random();
        }
        /// <summary>
        /// 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBegin_Click(object sender, EventArgs e)
        {
            step = 0;
            GetAllData();
            GetTakePoker();
        }

        /// <summary>
        /// 初始化357游戏数据（也可以通过抽象出从接口的方式去获取数据，这样数据就可以扩展）
        /// </summary>
        /// <returns></returns>
        private void GetAllData()
        {
            dicAll?.Clear();
            dicAll.Add(1, new List<int>() { 1, 2, 3 });
            dicAll.Add(2, new List<int>() { 4, 5, 6, 7, 8 });
            dicAll.Add(3, new List<int>() { 9, 10, 11, 12, 12, 14, 15 });
        }

        /// <summary>
        /// 拿火柴
        /// </summary>
        private async void GetTakePoker()
        {
            int userSelectedRow = ran.Next(1, dicAll.Count + 1);
            if (dicAll[userSelectedRow] == null || dicAll[userSelectedRow].Count == 0)
            {
                ShowLog($"所提供的源数据异常");
                return;
            }
            while (dicAll[userSelectedRow].Count > 0)
            {
                var taskList = new List<Task<bool>> { userOneTake(userSelectedRow), userTwoTake(userSelectedRow) }; //多线程 减少时间消耗
                await Task.WhenAll(taskList);
            }
        }
        /// <summary>
        /// 用户一拿火柴
        /// </summary>
        /// <param name="userOneSelectedRow"></param>
        /// <returns></returns>
        private Task<bool> userOneTake(int userOneSelectedRow)
        {
            return Task.Run(() =>
            {
                try
                {
                    GetPokerCaculate(userOneSelectedRow, Operation.userOne);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
        }
        /// <summary>
        /// 用户二拿火柴
        /// </summary>
        /// <param name="userOneSelectedRow"></param>
        /// <returns></returns>
        private Task<bool> userTwoTake(int userOneSelectedRow)
        {
            return Task.Run(() =>
            {
                try
                {
                    GetPokerCaculate(userOneSelectedRow, Operation.userTwo);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// 取火柴计算
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="operation"></param>
        private void GetPokerCaculate(int rowIndex, Operation operation)
        {
            lock (obj)
            {
                List<int> lstSelectedRowData = dicAll[rowIndex];
                if (lstSelectedRowData.Count == 0)
                {
                    return;
                }
                int selectCount = ran.Next(1, lstSelectedRowData.Count + 1);
                dicAll[rowIndex].RemoveRange(0, selectCount);
                var takeMsg = string.Empty;
                step++;
                if (operation == Operation.userOne)
                {
                    takeMsg = $"第{step} 步: userOne取了第{rowIndex}行{selectCount}根火柴";
                }
                else
                {
                    takeMsg = $"第{step} 步: userTwo取了第{rowIndex}行{selectCount}根火柴";
                }
                this.Invoke(new setTxtContent(ShowLog), new object[] { takeMsg });
                if (dicAll[rowIndex].Count == 0)//最后一个取完列表中的火柴判定为输
                {
                    string content = string.Empty;
                    if (operation == Operation.userOne)
                    {
                        content = "userOne lose";
                    }
                    else
                    {
                        content = "userTwo lose";
                    }
                    this.Invoke(new setTxtContent(ShowLog), new object[] { content });
                }
            }
        }

        /// <summary>
        /// 界面显示结果
        /// </summary>
        /// <param name="msg"></param>
        private void ShowLog(string msg)
        {
            if (!string.IsNullOrEmpty(txtInfo.Text))
            {
                txtInfo.AppendText("\r\n");
                txtInfo.AppendText("\r\n");
            }
            txtInfo.AppendText(msg);
        }

    }
    /// <summary>
    /// 操作人枚举
    /// </summary>
    public enum Operation
    {
        userOne,
        userTwo
    }
}
