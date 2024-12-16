using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharedClass
{
    public class FormBlockClass
    {
        public void BlockMapElements(Button[,] mapArray)
        {
            for (int i = 1; i < mapArray.GetLength(0); i++)
            {
                for (int j = 1; j < mapArray.GetLength(1); j++)
                {
                    mapArray[i, j].Enabled = false;
                }
            }
        }

        public void UnblockMapElements(Button[,] mapArray)
        {
            for (int i = 1; i < mapArray.GetLength(0); i++)
            {
                for (int j = 1; j < mapArray.GetLength(1); j++)
                {
                    mapArray[i, j].Enabled = true;
                }
            }
        }
    }
}