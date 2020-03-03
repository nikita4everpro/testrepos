using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1  
{
    public partial class Form1 : Form
    {
        Random rand = new Random();

        //Byte[] BinArr;

        int[] IP = new int[64];

        class Eobj
        {
            public int a;
            public int b;
            public Eobj()
            {
                a = -1;
                b = -1;
            }
        }
        Eobj[] E = new Eobj[32];

        Byte[] K = new Byte[8];
        int[] PC1 = new int[56];
        int[] PC2 = new int[48];
        int[] LS = new int[16];

        int[] STable = new int[16 * 4 * 8];
        int[] P = new int[32];

        Byte[] C = new Byte[8];

        int k;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //SetRandom();
            SetDefault();
            
        }



        Byte[] Encrypt(Byte[] s)
        {
            string rezult="";

            Byte[] BinArr = new Byte[s.Length];
            for (int i = 0; i < s.Length; i++) BinArr[i] = s[i];

            //textBox5.Text += "IN\r\n"+WriteBinaryInString(BinArr);

            for (int i = 0; i < (BinArr.Length +7) / 8; i++)
            {
                Byte[] Byte64bit = new Byte[8];
                for (int j = 0; j < 8; j++) Byte64bit[j] = BinArr[i * 8 + j];
                 
                //textBox5.Text += "   Block " + i.ToString() + "\r\n";
                //textBox5.Text += " original\r\n";
                //textBox5.Text += WriteBinaryInString(Byte64bit);

                // Перестановка
                Byte[] Byte64bit2=new Byte[8]; 
                Byte64bit.CopyTo(Byte64bit2,0);
                
                for (int j = 0; j < 64; j++)
                {
                    SetBit(Byte64bit, j, GetBit(Byte64bit2, IP[j]));
                   
                }

                // textBox5.Text += " do for \r\n";
                // textBox5.Text += WriteBinaryInString(Byte64bit);

                // циклы шифрования
                for (int j = 0; j < 16; j++)
                {
                    Byte[] L = new Byte[] { Byte64bit[0], Byte64bit[1], Byte64bit[2], Byte64bit[3] };
                    Byte[] R = new Byte[] { Byte64bit[4], Byte64bit[5], Byte64bit[6], Byte64bit[7] };

                    // textBox5.Text += " Block " + j.ToString() + " L\r\n";
                    // textBox5.Text += WriteBinaryInString(L);
                    // 
                    // textBox5.Text += " Block " + j.ToString() + " R\r\n";
                    // textBox5.Text += WriteBinaryInString(R);

                    // Левая часть = R
                    Byte64bit[0] = R[0];
                    Byte64bit[1] = R[1];
                    Byte64bit[2] = R[2];
                    Byte64bit[3] = R[3];

                    // Правая часть = L xor fun(B,K)
                    Byte[] fR = new Byte[4];
                    fR = fun(R, GetKey(j));

                    Byte64bit[4] = (Byte)(L[0] ^ fR[0]);
                    Byte64bit[5] = (Byte)(L[1] ^ fR[1]);
                    Byte64bit[6] = (Byte)(L[2] ^ fR[2]);
                    Byte64bit[7] = (Byte)(L[3] ^ fR[3]);

                }

                // Меням местами
                Byte[] Byte64bit3 = new Byte[8];
                Byte64bit.CopyTo(Byte64bit3, 0);
                Byte64bit[0] = Byte64bit3[4];
                Byte64bit[1] = Byte64bit3[5];
                Byte64bit[2] = Byte64bit3[6];
                Byte64bit[3] = Byte64bit3[7];
                Byte64bit[4] = Byte64bit3[0];
                Byte64bit[5] = Byte64bit3[1];
                Byte64bit[6] = Byte64bit3[2];
                Byte64bit[7] = Byte64bit3[3];


                // textBox5.Text += " do obr perest \r\n";
                // textBox5.Text += WriteBinaryInString(Byte64bit);

                // Обртаная перестановка
                Byte64bit.CopyTo(Byte64bit2, 0);

                for (int j = 0; j < 64; j++)
                {
                    int num = -1;
                    for (int k = 0; k < 64; k++)
                    {
                        if (IP[k] == j)
                        {
                            num = k;
                            break;
                        }
                    }
                    SetBit(Byte64bit, j, GetBit(Byte64bit2, num));
                }

                // textBox5.Text += " obr perest \r\n";
                // textBox5.Text += WriteBinaryInString(Byte64bit);


                // Переносим в результат
                for (int j = 0; j < 8; j++) BinArr[i * 8 + j] = Byte64bit[j];
            }

            //textBox5.Text += "OUT\r\n" + WriteBinaryInString(BinArr);
            //textBox2bin.Text += WriteBinaryInString(BinArr);

            //rezult += BinaryToString(BinArr);
            return BinArr;
        }

        Byte[] Decrypt(Byte[] s)
        {
            Byte[] BinArr = new Byte[s.Length];
            for (int i = 0; i < s.Length; i++) BinArr[i] = s[i];

            string rezult="";

            for (int i = 0; i < (BinArr.Length +7 ) / 8; i++)
            {
                Byte[] Byte64bit = new Byte[8];
                for (int j = 0; j < 8; j++) Byte64bit[j] = BinArr[i * 8 + j];

                //textBox5.Text += "   Block " + i.ToString() +"\r\n";
                //textBox5.Text +=" decrypt orig \r\n" + WriteBinaryInString(Byte64bit);

                // Перестановка
                Byte[] Byte64bit2 = new Byte[8];
                Byte64bit.CopyTo(Byte64bit2, 0);
                for (int j = 0; j < 64; j++)
                {
                    SetBit(Byte64bit, j, GetBit(Byte64bit2, IP[j]));
                }
                //textBox5.Text += " perest \r\n";
                //textBox5.Text += WriteBinaryInString(Byte64bit);

                // Меням местами
                Byte[] Byte64bit3 = new Byte[8];
                Byte64bit.CopyTo(Byte64bit3, 0);
                Byte64bit[0] = Byte64bit3[4];
                Byte64bit[1] = Byte64bit3[5];
                Byte64bit[2] = Byte64bit3[6];
                Byte64bit[3] = Byte64bit3[7];
                Byte64bit[4] = Byte64bit3[0];
                Byte64bit[5] = Byte64bit3[1];
                Byte64bit[6] = Byte64bit3[2];
                Byte64bit[7] = Byte64bit3[3];

                // циклы шифрования
                for (int j = 15; j >=0; j--)
                {
                    Byte[] L = new Byte[] { Byte64bit[0], Byte64bit[1], Byte64bit[2], Byte64bit[3] };
                    Byte[] R = new Byte[] { Byte64bit[4], Byte64bit[5], Byte64bit[6], Byte64bit[7] };

                    //textBox5.Text += " Block " + i.ToString() + " L\r\n";
                    //textBox5.Text += WriteBinaryInString(L);
                    //
                    //textBox5.Text += " Block " + i.ToString() + " R\r\n";
                    //textBox5.Text += WriteBinaryInString(R);

                    // Левая часть = R
                    Byte64bit[4] = L[0];
                    Byte64bit[5] = L[1];
                    Byte64bit[6] = L[2];
                    Byte64bit[7] = L[3];

                    // Правая часть = L xor fun(B,K)
                    Byte[] fR = new Byte[4];
                    fR = fun(L, GetKey(j));

                    Byte64bit[0] = (Byte)(R[0] ^ fR[0]);
                    Byte64bit[1] = (Byte)(R[1] ^ fR[1]);
                    Byte64bit[2] = (Byte)(R[2] ^ fR[2]);
                    Byte64bit[3] = (Byte)(R[3] ^ fR[3]);

                }
                
                // textBox5.Text += " after obr for \r\n";
                // textBox5.Text += WriteBinaryInString(Byte64bit);

                // Обртаная перестановка
                Byte64bit.CopyTo(Byte64bit2, 0);

                for (int j = 0; j < 64; j++)
                {
                    int num = -1;
                    for (int k = 0; k < 64; k++)
                    {
                        if (IP[k] == j)
                        {
                            num = k;
                            break;
                        }
                    }
                    SetBit(Byte64bit, j, GetBit(Byte64bit2, num));
                }

                //textBox5.Text += " obr perest \r\n";
                //textBox5.Text += WriteBinaryInString(Byte64bit);


                // Переносим в результат
                for (int j = 0; j < 8; j++) BinArr[i * 8 + j] = Byte64bit[j];

            }

            //textBox3bin.Text += WriteBinaryInString(BinArr);

            //rezult += BinaryToString(BinArr);
            return BinArr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox2bin.Text = "";

            //textBox5.Text = "";

            textBox2.Text += CBCEncrypt(BinaryStringToBinary(textBox1bin.Text));
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            textBox3bin.Text = "";

            textBox3.Text += CBCDecrypt(BinaryStringToBinary(textBox2bin.Text));
        }

        Byte[] StringToBinary(string s)
        {
            /*
            // Дополняем пробелами для равных блоков
            int add = 4 - s.Length % 4;
            if(add!=4) for (int i = 0; i < add; i++) s += " ";
            */

            Byte[] ByteArray = System.Text.Encoding.Unicode.GetBytes(s);
            
            return ByteArray;
        }

        string WriteBinaryInString(Byte[] b)
        {
            string s = "";
            Byte[] b2 = new Byte[b.Length];
            b.CopyTo(b2, 0);
            for (int i = 0; i < b.Length; i++)
            {
                string s2 = "";
                for (int j = 0; j < 8; j++)
                {
                    s2 = (b2[i] % 2).ToString() + s2;
                    b2[i] /= 2;
                }
                s += s2 + "\r\n";
            }
            return s;
        }

        string BinaryToString(Byte[] b)
        {
            string s = "";

            s=System.Text.Encoding.Unicode.GetString(b);

            return s;
        }

        int GetBit(Byte[] b,int n)
        {
            return (b[n / 8] >> (7 - n % 8)) & 1;
        }

        void SetBit(Byte[] b, int n, int bit)
        {
            if(bit==0)
            {
                unchecked
                {
                    b[n / 8] &= (Byte)~(1 << (7 - (n % 8)));
                }
            }
            else if(bit==1)
            {
                b[n / 8] |= (Byte) (1 << (7- (n % 8)));
            }
        }

        Byte[] fun(Byte[] b, Byte[] key)
        {
            Byte[] rez = new Byte[4];

            //textBox5.Text += "до " + "\r\n" + WriteBinaryInString(b) + "\r\n";

            // Расширение
            Byte[] rasshir = new Byte[6];
            for (int j = 0; j < 32; j++)
            {
                SetBit(rasshir, E[j].a, GetBit(b, j));
                if (E[j].b != -1)
                {
                    SetBit(rasshir, E[j].b, GetBit(b, j));
                }
            }
            //textBox5.Text += "расш " + "\r\n" + WriteBinaryInString(rasshir) + "\r\n";
            //textBox5.Text += "key  " + "\r\n" + WriteBinaryInString(key) + "\r\n";

            // xor
            for (int i = 0; i < 6; i++)
            {
                rasshir[i] ^= key[i];
            }
            //textBox5.Text += "xor  " + "\r\n" + WriteBinaryInString(rasshir) + "\r\n";

            // S
            Byte[] S = new Byte[8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    SetBit(S, j + i * 8, GetBit(rasshir, j + i * 6));
                }
            }
            //textBox5.Text += "S  " + "\r\n" + WriteBinaryInString(S) + "\r\n";

            for (int i = 0; i < 8; i++)
            {
                int k = GetBit(S, i * 8 + 0) * 2 + GetBit(S, i * 8 + 5) * 1;
                int l = GetBit(S, i * 8 + 1) * 8 + GetBit(S, i * 8 + 2) * 4 +GetBit(S, i * 8 + 3) * 2 + GetBit(S, i * 8 + 4) * 1;


                int fromSTable = STable[i * 4 * 16 + k * 16 + l];

                //textBox5.Text += i.ToString() + " " + k.ToString() + " " + l.ToString() + " - " + fromSTable.ToString() + "   ";

                for (int j = 3; j >= 0; j--)
                {
                    SetBit(rez, i * 4 + j, fromSTable % 2);

                    //textBox5.Text += (fromSTable % 2).ToString();
                    fromSTable /= 2;
                }

                //textBox5.Text += "\r\n";
            }

            //textBox5.Text += "rez do  "  + WriteBinaryInString(rez) + "\r\n";

            // Перестановка
            Byte[] rez2 = new Byte[4];
            rez.CopyTo(rez2, 0);
            for (int j = 0; j < 32; j++)
            {
                SetBit(rez, j, GetBit(rez2, P[j]));
            }

            //textBox5.Text += "rez  " + WriteBinaryInString(rez) + "\r\n";
            

            return rez;
        }

        Byte[] GetKey(int n)
        {
            Byte[] rez = new Byte[6];

            // Перестановка выбор
            Byte[] perest = new Byte[7];
            for (int j = 0; j < 56; j++)
            {
                SetBit(perest, j, GetBit(K, PC1[j]));
            }
            //textBox5.Text += "perest " + WriteBinaryInString(perest) + "\r\n";

            // L R
            Byte[] L = new Byte[4];
            Byte[] R = new Byte[4];
            for(int i=0;i<28;i++)
            {
                SetBit(L, i, GetBit(perest, i));
                SetBit(R, i, GetBit(perest, i+28));
            }
            //textBox5.Text += "L " + WriteBinaryInString(L) + "\r\n";
            //textBox5.Text += "R " + WriteBinaryInString(R) + "\r\n";

            for(int i=0;i<n+1;i++)
            {
                Byte[] Lold = new Byte[4];
                Byte[] Rold = new Byte[4];
                L.CopyTo(Lold, 0);
                R.CopyTo(Rold, 0);

                for (int j = 0; j < 28; j++)
                {
                    int oldnum = j + LS[i];
                    if (oldnum >= 28) oldnum -= 28;

                    SetBit(L, j, GetBit(Lold, oldnum));
                    SetBit(R, j, GetBit(Rold, oldnum));
                }

                //textBox5.Text += "R" + i + " " + WriteBinaryInString(R) + "\r\n";
                //textBox5.Text += "L" + i + " " + WriteBinaryInString(L) + "\r\n";
                
            }

            // Объединение L и R
            Byte[] doperest = new Byte[7];
            for (int i = 0; i < 28; i++)
            {
                SetBit(doperest, i + 28, GetBit(L, i));
                SetBit(doperest, i, GetBit(R, i));
            }
            //textBox5.Text += "do " + WriteBinaryInString(doperest) + "\r\n";

            // Перестановка
            for (int i = 0; i < 48; i++)
            {
                SetBit(rez, i, GetBit(doperest, PC2[i]));
            }
            //textBox5.Text += "posle " + WriteBinaryInString(rez) + "\r\n";

            return rez;
        }

        void SetRandom()
        {
            // IP
            for (int i = 0; i < IP.Length; i++) IP[i] = i;
            for (int i = 0; i < IP.Length; i++)
            {
                int num = rand.Next(0, IP.Length);
                int c = IP[i];
                IP[i] = IP[num];
                IP[num] = c;
            }
            //for (int i = 0; i < IP.Length; i++) textBox5.Text+= i+ " - "+ IP[i].ToString()+"\r\n";

            // E
            for (int i = 0; i < E.Length; i++) E[i] = new Eobj();
            bool second = false;
            for (int i = 0; i < 48; i++)
            {
                int num = rand.Next(0, E.Length);
                int numcheck = num;
                while (true)
                {
                    if (E[num].a == -1)
                    {
                        E[num].a = i;
                        break;
                    }
                    else if (second && E[num].b == -1)
                    {
                        E[num].b = i;
                        break;
                    }
                    num++;
                    if (num >= E.Length) num = 0;
                    if (num == numcheck) second = true;
                }
            }
            //for (int i = 0; i < E.Length; i++) textBox5.Text += i + " - " + E[i].a.ToString() +", "+ E[i].b.ToString() + "\r\n";

            // K
            
            for(int i=0;i<8;i++)
            {
                int checkbit=0;
                for(int j=0;j<7;j++)
                {
                    int randnext = rand.Next(0, 2);
                    SetBit(K, i * 8 + j, randnext);
                    checkbit ^= randnext;
                }
                SetBit(K, i * 8 + 7, checkbit);
            }

            textBox5.Text += "K " + WriteBinaryInString(K) + "\r\n";



            // PC1
            for (int i = 0; i < PC1.Length; i++) PC1[i] = i + ((i) / 7);
            for (int i = 0; i < PC1.Length; i++)
            {
                int num = rand.Next(0, PC1.Length);
                int c = PC1[i];
                PC1[i] = PC1[num];
                PC1[num] = c;
            }
            //for (int i = 0; i < PC.Length; i++) textBox5.Text += i + " - " + PC[i].ToString() + "\r\n";

            // PC2
            for (int i = 0; i < PC2.Length; i++)
            {
                bool again = true;
                while (again)
                {
                    PC2[i] = rand.Next(0, 56);

                    again = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (PC2[j] == PC2[i]) again = true;
                    }
                }
            }
            //for (int i = 0; i < PC2.Length; i++) textBox5.Text += i + " - " + PC2[i].ToString() + "\r\n";

            // LS
            for (int i = 0; i < LS.Length; i++)
            {
                LS[i] = rand.Next(1, 3);
            }
            //for (int i = 0; i < LS.Length; i++) textBox5.Text += i + " - " + LS[i].ToString() + "\r\n";

            // S table
            for (int i = 0; i < 4 * 8; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    STable[i * 16 + j] = j;
                }
                // Перемешивание
                for (int k = 0; k < 16; k++)
                {
                    int num = rand.Next(0, 16);
                    int c = STable[i * 16 + k];
                    STable[i * 16 + k] = STable[i * 16 + num];
                    STable[i * 16 + num] = c;
                }
            }
            /*
            for (int i = 0; i < STable.Length; i++)
            {
                textBox5.Text +=  STable[i].ToString() + " ";
                if (i % 16 == 15) textBox5.Text += "\r\n";
            }*/

            // P
            for (int i = 0; i < P.Length; i++) P[i] = i;
            for (int i = 0; i < P.Length; i++)
            {
                int num = rand.Next(0, P.Length);
                int c = P[i];
                P[i] = P[num];
                P[num] = c;
            }
            //for (int i = 0; i < P.Length; i++) textBox5.Text += i + " - " + P[i].ToString() + "\r\n";


            Byte[] K0 = GetKey(15);
            textBox5.Text += "K0 " + WriteBinaryInString(K0) + "\r\n";
        }

        void SetDefault()
        {
            // IP
            IP = new int[] {
                58,50,42,34,26,18,10,2,60,52,44,36,28,20,12,4,
                62,54,46,38,30,22,14,6,64,56,48,40,32,24,16,8,
                57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,
                61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7
            };
            for (int i = 0; i < IP.Length; i++) IP[i]--;
            //for (int i = 0; i < IP.Length; i++) textBox5.Text+= i+ " - "+ IP[i].ToString()+"\r\n";

            // E
            for (int i = 0; i < E.Length; i++) E[i] = new Eobj();
            E[0].a = 2; E[0].b = 48;
            E[1].a = 3; E[1].b = -1;
            E[2].a = 4; E[2].b = -1;
            E[3].a = 5; E[3].b = 7;
            E[4].a = 6; E[4].b = 8;
            E[5].a = 9; E[5].b = -1;
            E[6].a = 10; E[6].b = -1;
            E[7].a = 11; E[7].b = 13;
            E[8].a = 12; E[8].b = 14;
            E[9].a = 15; E[9].b = -1;
            E[10].a = 16; E[10].b = -1;
            E[11].a = 17; E[11].b = 19;
            E[12].a = 18; E[12].b = 20;
            E[13].a = 21; E[13].b = -1;
            E[14].a = 22; E[14].b = -1;
            E[15].a = 23; E[15].b = 25;
            E[16].a = 24; E[16].b = 26;
            E[17].a = 27; E[17].b = -1;
            E[18].a = 28; E[18].b = -1;
            E[19].a = 29; E[19].b = 31;
            E[20].a = 30; E[20].b = 32;
            E[21].a = 33; E[21].b = -1;
            E[22].a = 34; E[22].b = -1;
            E[23].a = 35; E[23].b = 37;
            E[24].a = 36; E[24].b = 38;
            E[25].a = 39; E[25].b = -1;
            E[26].a = 40; E[26].b = -1;
            E[27].a = 41; E[27].b = 43;
            E[28].a = 42; E[28].b = 44;
            E[29].a = 45; E[29].b = -1;
            E[30].a = 46; E[30].b = -1;
            E[31].a = 1; E[31].b = 47;
            for (int i = 0; i < E.Length; i++)
            {
                E[i].a--;
                if (E[i].b != -1) E[i].b--;
            }
            //for (int i = 0; i < E.Length; i++) textBox5.Text += i + " - " + E[i].a.ToString() +", "+ E[i].b.ToString() + "\r\n";

            // K
            // 00011111 00011111 00011111 00011111 00001110 00001110 00001110 00001110
            /*for (int i=3;i<32;i+=8) SetBit(K, i, 1);

            for (int i = 4; i < 32; i += 8)
                for (int j = 0; j < 4; j++) SetBit(K, i + j, 1);

            for (int i = 36; i < 64; i += 8)
                for (int j = 0; j < 3; j++) SetBit(K, i + j, 1);*/

            // 0101010101010101
            //for (int i = 7; i < 64; i += 8) SetBit(K, i, 1);

            // random
            for (int i = 0; i < 8; i++)
            {
                int checkbit = 0;
                for (int j = 0; j < 7; j++)
                {
                    int randnext = rand.Next(0, 2);
                    SetBit(K, i * 8 + j, randnext);
                    checkbit ^= randnext;
                }
                SetBit(K, i * 8 + 7, checkbit);
            }

            textBox5.Text += "K " + WriteBinaryInString(K) + "\r\n";



            // PC1
            PC1 = new int[]
            {
                57,49,41,33,25,17,9, 1, 58,50,42,34,26,18,
                10,2, 59,51,43,35,27,19,11,3, 60,52,44,36,
                63,55,47,39,31,23,15,7, 62,54,46,38,30,22,
                14,6, 61,53,45,37,29,21,13,5, 28,20,12,4
            };
            for (int i = 0; i < PC1.Length; i++) PC1[i]--;
            //for (int i = 0; i < PC1.Length; i++) textBox5.Text += i + " - " + PC1[i].ToString() + "\r\n";

            // PC2
            PC2 = new int[]
            {
                14,17,11,24,1, 5, 3, 28,15,6, 21,10,23,19,12,4,
                26,8, 16,7, 27,20,13,2, 41,52,31,37,47,55,30,40,
                51,45,33,48,44,49,39,56,34,53,46,42,50,36,29,32
            };
            for (int i = 0; i < PC2.Length; i++) PC2[i]--;
            //for (int i = 0; i < PC2.Length; i++) textBox5.Text += i + " - " + PC2[i].ToString() + "\r\n";

            // LS
            LS = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
            //for (int i = 0; i < LS.Length; i++) textBox5.Text += i + " - " + LS[i].ToString() + "\r\n";

            // S table
            STable = new int[]
            {
                14,4, 13,1, 2, 15,11,8, 3, 10,6, 12,5, 9, 0, 7   ,
                0, 15,7, 4, 14,2, 13,1, 10,6, 12,11,9, 5, 3, 8   ,
                4, 1, 14,8, 13,6, 2, 11,15,12,9, 7, 3, 10,5, 0   ,
                15,12,8, 2, 4, 9, 1, 7, 5, 11,3, 14,10,0, 6, 13  ,

                15,1, 8, 14,6, 11,3, 4, 9, 7, 2, 13,12,0, 5, 10  ,
                3, 13,4, 7, 15,2, 8, 14,12,0, 1, 10,6, 9, 11,5   ,
                0, 14,7, 11,10,4, 13,1, 5, 8, 12,6, 9, 3, 2, 15  ,
                13,8, 10,1, 3, 15,4, 2, 11,6, 7, 12,0, 5, 14,9   ,

                10,0, 9, 14,6, 3, 15,5, 1, 13,12,7, 11,4, 2, 8   ,
                13,7, 0, 9, 3, 4, 6, 10,2, 8, 5, 14,12,11,15,1   ,
                13,6, 4, 9, 8, 15,3, 0, 11,1, 2, 12,5, 10,14,7   ,
                1, 10,13,0, 6, 9, 8, 7, 4, 15,14,3, 11,5, 2, 12  ,

                7, 13,14,3, 0, 6, 9, 10,1, 2, 8, 5, 11,12,4, 15  ,
                13,8, 11,5, 6, 15,0, 3, 4, 7, 2, 12,1, 10,14,9   ,
                10,6, 9, 0, 12,11,7, 13,15,1, 3, 14,5, 2, 8, 4   ,
                3, 15,0, 6, 10,1, 13,8, 9, 4, 5, 11,12,7, 2, 14  ,

                2, 12,4, 1, 7, 10,11,6, 8, 5, 3, 15,13,0, 14,9   ,
                14,11,2, 12,4, 7, 13,1, 5, 0, 15,10,3, 9, 8, 6   ,
                4, 2, 1, 11,10,13,7, 8, 15,9, 12,5, 6, 3, 0, 14  ,
                11,8, 12,7, 1, 14,2, 13,6, 15,0, 9, 10,4, 5, 3   ,

                12,1, 10,15,9, 2, 6, 8, 0, 13,3, 4, 14,7, 5, 11  ,
                10,15,4, 2, 7, 12,9, 5, 6, 1, 13,14,0, 11,3, 8   ,
                9, 14,15,5, 2, 8, 12,3, 7, 0, 4, 10,1, 13,11,6   ,
                4, 3, 2, 12,9, 5, 15,10,11,14,1, 7, 6, 0, 8, 13  ,

                4, 11,2, 14,15,0, 8, 13,3, 12,9, 7, 5, 10,6, 1   ,
                13,0, 11,7, 4, 9, 1, 10,14,3, 5, 12,2, 15,8, 6   ,
                1, 4, 11,13,12,3, 7, 14,10,15,6, 8, 0, 5, 9, 2   ,
                6, 11,13,8, 1, 4, 10,7, 9, 5, 0, 15,14,2, 3, 12  ,

                13,2, 8, 4, 6, 15,11,1, 10,9, 3, 14,5, 0, 12,7   ,
                1, 15,13,8, 10,3, 7, 4, 12,5, 6, 11,0, 14,9, 2   ,
                7, 11,4, 1, 9, 12,14,2, 0, 6, 10,13,15,13,5, 8   ,
                2, 1, 14,7, 4, 10,8, 13,15, 12,9, 0, 3, 5, 6, 11
            };
            /*
            for (int i = 0; i < STable.Length; i++)
            {
                textBox5.Text +=  STable[i].ToString() + " ";
                if (i % 16 == 15) textBox5.Text += "\r\n";
            }*/

            // P
            P = new int[]
            {
                16,7, 20,21,29,12,28,17,1, 15,23,26,5, 18,31,10,
                2, 8, 24,14,32,27,3, 9, 19,13,30,6, 22,11,4, 25
            };
            for (int i = 0; i < P.Length; i++) P[i]--;
            //for (int i = 0; i < P.Length; i++) textBox5.Text += i + " - " + P[i].ToString() + "\r\n";


            // Byte[] K0 = GetKey(15);
            // textBox5.Text += "K0 " + WriteBinaryInString(K0) + "\r\n";

            // C
            for (int i = 0; i < C.Length; i++) C[i] = (byte)CodeFunc(i);
            textBox5.Text += "C " + WriteBinaryInString(C) + "\r\n";

            // k
            k = 7;
            textBox5.Text += "k " + k + "\r\n";
        }



        string WriteBinaryStringInString(string s)
        {
            string outstring="";
            
            string s2="";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '\r' && s[i] != '\n')
                {
                    s2 += s[i];
                }
            }

            Byte[] bt = new Byte[s2.Length / 8];
            for (int i = 0; i < bt.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (s2[i * 8 + j] == '1')
                        SetBit(bt, i * 8 + j, 1);
                    else
                        SetBit(bt, i * 8 + j, 0);

                }
            }
            

            outstring= System.Text.Encoding.Unicode.GetString(bt);

            return outstring;
        }

        Byte[] BinaryStringToBinary(string s)
        {

            string s2 = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != '\r' && s[i] != '\n')
                {
                    s2 += s[i];
                }
            }

            Byte[] bt = new Byte[s2.Length / 8];
            for (int i = 0; i < bt.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (s2[i * 8 + j] == '1')
                        SetBit(bt, i * 8 + j, 1);
                    else
                        SetBit(bt, i * 8 + j, 0);

                }
            }
            
            return bt;
        }


        private void textBox1bin_Leave(object sender, EventArgs e)
        {
            textBox1.Text = WriteBinaryStringInString(textBox1bin.Text);

            //textBox5.Text += WriteBinaryStringInString(textBox1bin.Text);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1bin.Text = WriteBinaryInString(StringToBinary(textBox1.Text));
        }

        private void textBox2bin_Leave(object sender, EventArgs e)
        {
            textBox2.Text = WriteBinaryStringInString(textBox2bin.Text);

            //textBox5.Text += WriteBinaryStringInString(textBox1bin.Text);
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2bin.Text = WriteBinaryInString(StringToBinary(textBox2.Text));
        }



        string CBCEncrypt(Byte[] s)
        {
            Byte[] BinArr = s;
            Byte[] rezult=new Byte[s.Length];

            Byte[] C_local = new Byte[8];
            for (int j = 0; j < 8; j++) C_local[j] = C[j];
            //textBox5.Text += "IN\r\n"+WriteBinaryInString(BinArr);

            for (int i = 0; i < BinArr.Length*8; i+=k)
            {
                //textBox5.Text += "Входной блок\r\n" + WriteBinaryInString(C_local);

                // DES
                Byte[] rezblock = Encrypt(C_local);
                
                // Shift
                for (int j = 0; j < 64 - k; j++)
                {
                    SetBit(C_local, j, GetBit(C_local, j + k));
                }

                //textBox5.Text += "Сдвиг\r\n" + WriteBinaryInString(C_local);
                //textBox5.Text += "Выходной блок\r\n" + WriteBinaryInString(rezblock);

                // ШТ
                //textBox5.Text += "ШТ\r\n";
                for (int j = 0; j < k && (i + j) < BinArr.Length * 8; j++)
                {
                    int bit = GetBit(rezblock, j) ^ GetBit(BinArr, i + j);

                    SetBit(rezult, i+j, bit);

                    SetBit(C_local, 64 - k + j, GetBit(rezblock, j));

                    //textBox5.Text += bit;
                }
                //textBox5.Text += "\r\n";



            }

            textBox2bin.Text += WriteBinaryInString(rezult);

            return BinaryToString(rezult);
        }

        string CBCDecrypt(Byte[] s)
        {
            Byte[] BinArr = s;
            Byte[] rezult = new Byte[s.Length];

            Byte[] C_local = new Byte[8];
            for (int j = 0; j < 8; j++) C_local[j] = C[j];
            //textBox5.Text += "IN\r\n"+WriteBinaryInString(BinArr);

            for (int i = 0; i < BinArr.Length * 8; i += k)
            {
                // DES
                Byte[] rezblock = Encrypt(C_local);

                // Shift
                for (int j = 0; j < 64 - k; j++)
                {
                    SetBit(C_local, j, GetBit(C_local, j + k));
                }
                
                // ШТ
                for (int j = 0; j < k && (i + j) < BinArr.Length * 8; j++)
                {
                    int bit = GetBit(rezblock, j) ^ GetBit(BinArr, i + j);
                    SetBit(rezult, i + j, bit);

                    SetBit(C_local, 64 - k + j, GetBit(rezblock, j));
                }
                

            }

            textBox3bin.Text += WriteBinaryInString(rezult);

            return BinaryToString(rezult);
        }


        int T0 = 11;
        int A = 41;
        int B = 256;
        int C_ = 13;

        int CodeFunc(int i)
        {
            if (i == 0) return T0;
            else return (A * CodeFunc(i - 1) + C_) % B;
        }

        private void ButtonOpenCont_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory= Environment.CurrentDirectory;
            openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            System.IO.Stream str=  openFileDialog1.OpenFile();
            byte[] buffer = new byte[str.Length];
            str.Read(buffer, 0, (int)str.Length);
            TextBoxCont.Text = System.Text.Encoding.Unicode.GetString(buffer);
            str.Close();
        }
    }
}
