using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            AllowDrop = true;
            DragEnter += (sdr, ea) => ea.Effect = DragDropEffects.All;
            DragDrop += (sdr, ea) => {
                var fname = ((string[])(ea.Data.GetData(DataFormats.FileDrop))).FirstOrDefault();
                pictureBox1.Image = DdsLoader.LoadFromFile(fname);
                if(pictureBox1.Image == null)pictureBox1.Image = new Bitmap(fname);
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            // フィルタ用のカーネル
            //const int kernelSize = 3; // カーネルサイズ
            //double[,] kernel = new double[kernelSize, kernelSize]{
            //                    {1/16.0, 1/8.0, 1/16.0},
            //                    {1/8.0,  1/4.0, 1/8.0},
            //                    {1/16.0, 1/8.0, 1/16.0}};

            const int kernelSize = 3;
            double[,] kernel = new double[kernelSize, kernelSize]{
                                {1.0/9.0,1.0/9.0,1.0/9.0},
                                {1.0/9.0,1.0/9.0,1.0/9.0},
                                {1.0/9.0,1.0/9.0,1.0/9.0}};

            Bitmap bitmap = (Bitmap)pictureBox1.Image;
            var width = bitmap.Width;
            var height = bitmap.Height;
            Bitmap bitmap1 = new Bitmap(width, height);
            // 画像処理
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int sumR = 0;
                    int sumG = 0;
                    int sumB = 0;
                    for (int k = -kernelSize / 2; k <= kernelSize / 2; k++)
                    {
                        for (int n = -kernelSize / 2; n <= kernelSize / 2; n++)
                        {
                            var color = bitmap.GetPixel(j, i);
                            if (j + n >= 0 && j + n < width && i + k >= 0 && i + k < height)
                            {
                                sumR += (int)(color.R * kernel[n + kernelSize / 2, k + kernelSize / 2]);
                                sumG += (int)(color.G * kernel[n + kernelSize / 2, k + kernelSize / 2]);
                                sumB += (int)(color.B * kernel[n + kernelSize / 2, k + kernelSize / 2]);
                            }
                        }
                    }
                    sumR = Byte2Int(sumR);
                    sumG = Byte2Int(sumG);
                    sumB = Byte2Int(sumB);
                    bitmap1.SetPixel(j, i, Color.FromArgb(sumR, sumG, sumB));
                }
            }

            pictureBox1.Image = bitmap1;
        }

        private int Byte2Int(int num) 
        {
            if (num > 255) return 255;
            else if (num < 0) return 0;
            return num;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            Bitmap bitmap = (Bitmap)pictureBox1.Image;   
            var width = bitmap.Width;
            var height = bitmap.Height;
            Int32 split = (Int32)numericUpDown1.Value;
            Bitmap bitmap1 = new Bitmap(width, height);
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    var color = bitmap.GetPixel(i, j);
                    int[] colors = new int[3];
                    colors[0] = color.R;
                    colors[1] = color.G;
                    colors[2] = color.B;
                    for (int k = 0; k < split; ++k)
                    {
                        int col1 = k * (255 / split);
                        int col2 = (k + 1) * (255 / split);
                        
                        if (col1 <= color.R && color.R <= col2)
                        {
                            colors[0] = (col1 + col2) / 2;
                        }
                        if (col1 <= color.G && color.G <= col2)
                        {
                            colors[1] = (col1 + col2) / 2;
                        }
                        if (col1 <= color.B && color.B <= col2)
                        {
                            colors[2] = (col1 + col2) / 2;
                        }
                    }
                    bitmap1.SetPixel(i, j, Color.FromArgb(colors[0], colors[1], colors[2]));
                }
            }
            
            pictureBox1.Image = bitmap1;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }

        private void numericUpDown3_ValueChanged_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            Bitmap bitmap = (Bitmap)pictureBox1.Image;
            bitmap.Save("C://Users//hakas//Desktop//kanae2.bmp");
        }
    }

    public class DdsSimpleFileInfo
    {
        /// <summary>
        /// この情報が有効であることを示す値を取得または設定します。
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 画像の横幅を取得または設定します。
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 画像の高さを取得または設定します。
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// ピクセルデータの開始位置を取得または設定します。
        /// </summary>
        public int PixelOffset { get; set; }

        /// <summary>
        /// ファイル形式を示す文字列を取得または設定します。
        /// </summary>
        public string Format { get; set; }
    }

    public static class BinaryUtility
    {
        /// <summary>
        /// 指定したバイト列のオフセット位置に格納されている、32ビット無符号整数を取得します。
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static uint MakeUInt32(byte[] bytes, int offset)
        {
            return MakeUInt32(bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3]);
        }

        public static uint MakeUInt32(byte a, byte b, byte c, byte d)
        {
            return (uint)((a) | ((b) << 8) | ((c) << 16) | ((d) << 24));
        }

        /// <summary>
        /// 指定したバイト列のオフセット位置に格納されている、16ビット無符号整数を取得します。
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ushort MakeUInt16(byte[] bytes, int offset)
        {
            return (ushort)((bytes[offset]) | ((bytes[offset + 1]) << 8));
        }
    }

    public struct BlockInfo
    {
        public int X;        // ピクセル位置X
        public int Y;        // ピクセル位置Y
        public int Offset;    // ブロックデータオフセット

        public BlockInfo(int x, int y, int offset)
        {
            X = x;
            Y = y;
            Offset = offset;
        }
    }

    public static class DXTUtility
    {
        /// <summary>
        /// 指定されたピクセルサイズを格納するために必要なブロックの個数を取得します。
        /// </summary>
        /// <param name="pixelSize">ピクセル数</param>
        /// <returns></returns>
        public static int GetBlockCount(int pixelSize)
        {
            return (pixelSize >> 2) + (((pixelSize & 3) != 0) ? 1 : 0);
        }

        /// <summary>
        /// 指定された横幅、高さの画像のブロック情報をすべて列挙します。
        /// </summary>
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        /// <param name="dataOffset">ファイル中のブロック先頭オフセット</param>
        /// <param name="blockSize">1ブロックのバイト数</param>
        /// <returns></returns>
        public static IEnumerable<BlockInfo> EnumerateBlocks(int width, int height, int dataOffset, int blockSize)
        {
            int horizontalBlockCount = GetBlockCount(width);
            int verticalBlockCount = GetBlockCount(height);

            int offset = dataOffset;
            for (int i = 0; i < verticalBlockCount; i++)
            {
                for (int j = 0; j < horizontalBlockCount; j++)
                {
                    yield return new BlockInfo(j * 4, i * 4, offset);
                    offset += blockSize;
                }
            }
        }

       
    }

    public struct ColorRgba
    {
        public float R; // 赤成分(0.0～1.0)
        public float G; // 緑成分(0.0～1.0)
        public float B; // 青成分(0.0～1.0)
        public float A; // アルファ成分(0.0～1.0)

        /// <summary>
        /// ColorRgba構造体の新しいインスタンスを初期化します。
        /// </summary>
        public ColorRgba(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// RGB565形式の16ビットカラーデータから ColorRgbaインスタンスを生成します。
        /// </summary>
        public static ColorRgba FromRgb565(ushort rgb565)
        {
            int r = (rgb565 >> 11) & 0x1f;
            int g = (rgb565 >> 5) & 0x3f;
            int b = (rgb565 >> 0) & 0x1f;
            return new ColorRgba(r * 1.0f / 31.0f, g * 1.0f / 63.0f, b * 1.0f / 31.0f, 1.0f);
        }

        /// <summary>
        /// 指定された2つのカラーをリニア補間して中間色を求めます。
        /// </summary>
        public static ColorRgba Lerp(ColorRgba x, ColorRgba y, float s)
        {
            float r = x.R + s * (y.R - x.R);
            float g = x.G + s * (y.G - x.G);
            float b = x.B + s * (y.B - x.B);
            float a = x.A + s * (y.A - x.A);
            return new ColorRgba(r, g, b, a);
        }
    }

    public static class DdsLoader
    {
        public static System.Drawing.Bitmap LoadFromFile(string filename)
        {
            // ファイルをバイト列に読み込む
            byte[] data = System.IO.File.ReadAllBytes(filename);

            // ヘッダを解析
            var info = GetDdsInfo(data);
            if (info.IsValid)
            {
                switch (info.Format)
                {
                    case "DXT1":
                        // DXT1形式なら解凍してBitmapに変換
                        return DXT1Decoder.CreateBitmap(data, info.Width, info.Height, info.PixelOffset);
                }
            }

            return null;
        }

        public static DdsSimpleFileInfo GetDdsInfo(byte[] data)
        {
            const int HeaderSize = 0x80;

            var info = new DdsSimpleFileInfo();    // 情報クラスを作成

            if (data.Length < HeaderSize)
            {
                return info;        // DDSとしてのヘッダサイズが足りない
            }

            var magic = Encoding.UTF8.GetString(data, 0, 4);        // data 配列の先頭から連続する4バイトを文字列に変換
            if (magic != "DDS ")
            {
                return info;        // DDSファイルではない
            }

            // 画像の横幅と高さ
            info.Height = (int)BinaryUtility.MakeUInt32(data, 0x0c);        // data 配列の 12バイト目から連続する4バイトをInt32に変換
            info.Width = (int)BinaryUtility.MakeUInt32(data, 0x10);         // data 配列の 16バイト目から連続する4バイトをInt32に変換

            info.PixelOffset = HeaderSize;        // ピクセルデータの開始位置

            // DXT1形式かどうかをチェック
            info.Format = Encoding.UTF8.GetString(data, 0x54, 4);
            var byte1 = data[0x54];
            var byte2 = data[0x55];
            var byte3 = data[0x56];
            var byte4 = data[0x57];
            switch (info.Format)
            {
                case "DXT1":
                    break;
                default:
                    return info;
            }

            info.IsValid = true;            // ヘッダ取得成功を示すフラグを設定

            return info;
        }
    }

    public static class DXT1Decoder
    {
        public static System.Drawing.Bitmap CreateBitmap(byte[] data, int width, int height, int pixelOffset)
        {
            int BlockSize = 8;

            int physicalWidth = DXTUtility.GetBlockCount(width) * 4;
            int physicalHeight = DXTUtility.GetBlockCount(height) * 4;

            var pixels = new byte[physicalWidth * physicalHeight * 4];

            var blocks = DXTUtility.EnumerateBlocks(width, height, pixelOffset, BlockSize);

            foreach (var block in blocks)
            {
                // ここで block をデコードし pixel 配列の該当位置にRGBAデータを展開する
                ushort color0 = BinaryUtility.MakeUInt16(data, block.Offset);
                ushort color1 = BinaryUtility.MakeUInt16(data, block.Offset + 2);
                var colors = new ColorRgba[4];

                if (color0 > color1)
                {
                    // アルファなし。使える色は4色
                    colors[0] = ColorRgba.FromRgb565(color0);                            // カラー0そのまま
                    colors[1] = ColorRgba.FromRgb565(color1);                            // カラー1そのまま
                    colors[2] = ColorRgba.Lerp(colors[0], colors[1], 1.0f / 3.0f);       // color_2 = 2/3*color_0 + 1/3*color_1
                    colors[3] = ColorRgba.Lerp(colors[0], colors[1], 2.0f / 3.0f);       // color_3 = 1/3*color_0 + 2/3*color_1
                }
                else
                {
                    // アルファあり。使える色は3色
                    colors[0] = ColorRgba.FromRgb565(color0);                            // カラー0そのまま
                    colors[1] = ColorRgba.FromRgb565(color1);                            // カラー1そのまま
                    colors[2] = ColorRgba.Lerp(colors[0], colors[1], 1.0f / 2.0f);       // color_2 = 1/2*color_0 + 1/2*color_1
                    colors[3] = new ColorRgba(0, 0, 0, 0);                               // 透明
                }

                uint indexBits = BinaryUtility.MakeUInt32(data, block.Offset + 4);

                for (int y = 0; y < 4; y++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        var idx = indexBits & 0x03;                   // インデックスの取り出し
                        var col = colors[idx];                        // カラーテーブルからインデックスでカラーを取り出す
                        int xx = block.X + x;
                        int yy = block.Y + y;
                        int p = (xx + yy * physicalWidth) * 4;        // ピクセルの書き込み位置
                        pixels[p + 0] = (byte)(col.B * 255.0f);       // 青(0～255)
                        pixels[p + 1] = (byte)(col.G * 255.0f);       // 緑(0～255)
                        pixels[p + 2] = (byte)(col.R * 255.0f);       // 赤(0～255)
                        pixels[p + 3] = (byte)(col.A * 255.0f);       // アルファ(0～255)
                        indexBits >>= 2;                              // インデックスを2ビット右シフトして次に備える
                    }
                }
            }
            return CreateBitmap2(pixels, width, height, physicalWidth);
        }

        public static Bitmap CreateBitmap2(byte[] pixels, int width, int height, int physicalWidth)
        {
            if (width == physicalWidth)
            {
                // 作成したいビットマップの幅とストライドが同じ場合
                var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bd.Scan0, physicalWidth * 4 * height);
                bmp.UnlockBits(bd);
                return bmp;
            }
            else
            {
                // 作成したいビットマップの幅とストライドが異なる場合
                unsafe
                {
                    fixed (byte* ptr = &pixels[0])
                    {
                        // アンマネージドポインタを利用して作成したBitmapは fixed スコープ外に出るとAccess Violationを引き起こす
                        // そこで、別のBitmapにコピーしてそれを返す。元のBitmapは不要なので usingを使用してDisposeする。
                        using (var tmpbmp = new Bitmap(width, height, physicalWidth * 4, PixelFormat.Format32bppArgb, new IntPtr(ptr)))
                        {
                            var bmp = new Bitmap(tmpbmp);
                            return bmp;
                        }
                    }
                }
            }
        }

    }
}
