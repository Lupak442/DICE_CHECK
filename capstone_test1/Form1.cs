using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using Point = OpenCvSharp.Point;
using Tesseract;
using IronOcr;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;




namespace capstone_test1
{

    

    public partial class Form1 : Form
    {
        int n;
        private static System.Threading.Timer timer;

        public void Run()
        {
            VideoCapture video = new VideoCapture(0);

            Mat frame = new Mat();
            Mat gray = new Mat();
            Mat gaussian_blur = new Mat();
            Mat image_canny = new Mat();
            Mat binary = new Mat();
            Mat cut = new Mat();
            Bitmap bitmap;
            while (Cv2.WaitKey(33) != 'q')
            {

                video.Read(frame);

                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                Cv2.GaussianBlur(gray, gaussian_blur, new OpenCvSharp.Size(Convert.ToInt32(textBox3.Text), Convert.ToInt32(textBox3.Text)), 1, 1, BorderTypes.Default);
                Cv2.Threshold(gaussian_blur, binary, Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text), ThresholdTypes.Binary);
                cut = binary.SubMat(new OpenCvSharp.Rect(300, 300, 150, 100));
                bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(cut);
                

                var Ocr = new IronTesseract();
                var Input = new OcrInput(bitmap);
                
                    // Input.Deskew();  // use if image not straight
                    // Input.DeNoise(); // use if image contains digital noise
                    var result = Ocr.Read(Input);
                    //Console.WriteLine(result.Text);
                
                //Pix pix = PixConverter.ToPix(bitmap);
                //var engine = new TesseractEngine(@"C:\Users\user\source\repos\capstone_test1\capstone_test1\bin\Debug\tessdata", "eng", EngineMode.TesseractAndLstm);
                //var Result = engine.Process(pix);
                //Result.GetText();

                CircleSegment[] circles = Cv2.HoughCircles(gaussian_blur, HoughModes.Gradient, 1,
                         gaussian_blur.Rows / 20,  // change this value to detect circles with different distances to each other
                         150, 25, hScrollBar_min.Value, hScrollBar_max.Value // change the last two parameters
                                                                             // (min_radius & max_radius) to detect larger circles
                );
                n = circles.Length;
                for (int i = 0; i < circles.Length; i++)
                {
                    Point2f center = circles[i].Center;
                    Cv2.Circle(frame, (int)center.X, (int)center.Y, 1, new Scalar(0, 100, 100), 3, LineTypes.AntiAlias);
                    // circle outline                    
                    Cv2.Circle(frame, (int)center.X, (int)center.Y, (int)circles[i].Radius, new Scalar(255, 0, 255), 3, LineTypes.AntiAlias);
                }

                /*Cv2.Canny(gaussian_blur, image_canny, 100, 200, 3, true);
                Mat element = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(7, 7));
                Mat morph = new Mat();
                Cv2.MorphologyEx(image_canny, morph, MorphTypes.Close, element, iterations: 1);
                //Cv2.ImShow("mor", morph);
                ulong idx, ii;
                Point[][] contours;

                HierarchyIndex[] hierarchy;
                Cv2.FindContours(morph, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89KCOS);

                Mat contours_img = new Mat();
                Cv2.CvtColor(image_canny, contours_img, ColorConversionCodes.GRAY2BGR);
                for (idx = 0; idx < Convert.ToUInt32(contours.Length); idx++)
                {
                    RotatedRect rect = Cv2.MinAreaRect(contours[idx]);
                    double areaRatio = Math.Abs(Cv2.ContourArea(contours[idx]) / (rect.Size.Width * rect.Size.Height));
                    Cv2.DrawContours(contours_img, contours, Convert.ToInt32(idx), Scalar.Red, 2);

                }*/
                circle_min_value.Text = Convert.ToString(hScrollBar_min.Value);
                circle_max_value.Text = Convert.ToString(hScrollBar_max.Value);
                label1.Text = Convert.ToString(n);
                //label2.Text = result.GetText();
                label2.Text = result.Text;
                //label7.Text = Result.GetText();
                //Cv2.ImShow("con", contours_img);
                Cv2.ImShow("cut", cut);
                Cv2.ImShow("frame", frame);
                
            }
            frame.Dispose();
            video.Release();
            Cv2.WaitKey(0);
        }

            public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e) //영상시작
        {
            Run();


        }



        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        int count = 0;


        string id = null;
        string one_c = null;
        string two_c = null;
        string three_c = null;
        string four_c = null;
        string five_c = null;
        string six_c = null;
        private void button3_Click(object sender, EventArgs e) //눈금인식 끄기
        {
            
            if (count/6 == 0)
            {
                id = label2.Text;
            }
            if(count%6 == 0)
            {
                one_c = label1.Text;
            }
            if (count % 6 == 1)
            {
                two_c = label1.Text;
            }
            if (count % 6 == 2)
            {
                three_c = label1.Text;
            }
            if (count % 6 == 3)
            {
                four_c = label1.Text;
            }
            if (count % 6 == 4)
            {
                five_c = label1.Text;
            }
            if (count % 6 == 5)
            {
                six_c = label1.Text;
                dogs.Add(new Dog() { id = label2.Text, one = one_c, two = two_c, thr = three_c, four = four_c, five = five_c, six = six_c });
                count = 0;
            }


            textBox4.Text = Convert.ToString(count);
            count += 1;
            

        }
        List<Dog> dogs = new List<Dog>();
        private void button2_Click(object sender, EventArgs e) //데이터 저장
        {
             Excel.Application excelApp = null;
             Excel.Workbook workBook = null;
             Excel.Worksheet workSheet = null;

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);  // 바탕화면 경로
                string path = Path.Combine(desktopPath, "주사위판별검사데이터.xlsx");                              // 엑셀 파일 저장 경로

                excelApp = new Excel.Application();                             // 엑셀 어플리케이션 생성
                workBook = excelApp.Workbooks.Add();                            // 워크북 추가
                workSheet = workBook.Worksheets.get_Item(1) as Excel.Worksheet; // 엑셀 첫번째 워크시트 가져오기

                workSheet.Cells[1, 1] = "고유번호";
                workSheet.Cells[1, 2] = "1";
                workSheet.Cells[1, 3] = "2";
                workSheet.Cells[1, 4] = "3";
                workSheet.Cells[1, 5] = "4";
                workSheet.Cells[1, 6] = "5";
                workSheet.Cells[1, 7] = "6";

                // 엑셀에 저장할 개 데이터
                

                
                for (int i = 0; i < dogs.Count; i++)
                {
                    Dog dog = dogs[i];

                    // 셀에 데이터 입력
                    workSheet.Cells[2 + i, 1] = dog.id;
                    workSheet.Cells[2 + i, 2] = dog.one;
                    workSheet.Cells[2 + i, 3] = dog.two;
                    workSheet.Cells[2 + i, 4] = dog.thr;
                    workSheet.Cells[2 + i, 5] = dog.four;
                    workSheet.Cells[2 + i, 6] = dog.five;
                    workSheet.Cells[2 + i, 7] = dog.six;
                    workSheet.Cells[2 + i, 8] = dog.one+"+"+dog.thr;
                    workSheet.Cells[2 + i, 9] = dog.two + "+" + dog.four;
                    workSheet.Cells[2 + i, 10] = dog.five + "+" + dog.six;

                }

                workSheet.Columns.AutoFit();                                    // 열 너비 자동 맞춤
                workBook.SaveAs(path, Excel.XlFileFormat.xlWorkbookDefault);    // 엑셀 파일 저장
                workBook.Close(true);
                excelApp.Quit();
            }
            finally
            {
                ReleaseObject(workSheet);
                ReleaseObject(workBook);
                ReleaseObject(excelApp);
            }



        }
        static void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);  // 액셀 객체 해제
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();   // 가비지 수집
            }
        }



    }
    class Dog
    {
        public string id;
        public string one;     // 개 이름
        public string two;   // 개 종류
        public string thr;      // 개 성별
        public string four;
        public string five;
        public string six;
    }

}















