using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;

namespace Portal_Turret
{
    public partial class mainForm : Form
    {
        int RightVar, LeftVar, ControlVar = 0, TimeLen;
        int MoveMode = 1; //0 == open, 1 == close, 2 == pew, 3 == search
        Boolean Search = false;

        public mainForm()
        {
            InitializeComponent();

            DrawThing(0);
           // TestRun();

            CameraTest();
        }

        void TestRun()
        {
            OpenTurret();
            Search = true;
        }

        void PLaySound(int Type) //0 = open, 1 = close, 2 = searching, 3 = pew pew
        {
            SoundPlayer audio; Random Rand = new Random();
            String temp = "";
            if (Type == 0)
            {
                String First = "turret_active_", Second = "turret_deploy_"; 
                int randomInt = Rand.Next(1, 3);

                if (randomInt == 1)
                {
                    randomInt = Rand.Next(2, 9);
                    temp += First + randomInt.ToString();
                }
                else
                {
                    randomInt = Rand.Next(1, 7);
                    temp += Second + randomInt.ToString();
                }
            }
            else if (Type == 1)
            {
                //welp
                temp = "retract";
            }
            else if (Type == 2)
            {
                String First = "turret_autosearch_", Second = "turret_search_";
                int randomInt = Rand.Next(1, 3);

                if (randomInt == 1)
                {
                    randomInt = Rand.Next(1, 7);
                    temp += First + randomInt.ToString();
                }
                else
                {
                    randomInt = Rand.Next(1, 5);
                    temp += Second + randomInt.ToString();
                }

            }
            else if (Type == 3)
            {
                int randomInt = Rand.Next(1, 3);
                temp += "turret_fire_4x_0" + randomInt.ToString();
            }
            audio = new SoundPlayer(Properties.Resources.ResourceManager.GetStream(temp));

            audio.Play();
        }

        void OpenTurret()
        {
            TimeLen = 12;
            MoveMode = 0;
            movement.Enabled = true;
            PLaySound(0);
        }

        void CloseTurret()
        {
            TimeLen = 12;
            Search = false;
            MoveMode = 1;
            movement.Enabled = true;
            PLaySound(1);
        }

        private void movement_Tick(object sender, EventArgs e)
        {
            if (ControlVar == TimeLen)
            {
                ControlVar = 0;

                if (Search)
                {
                    TimeLen = 400;
                    if (MoveMode == 3)
                    {
                        CloseTurret();
                    }
                    else
                    {
                        MoveMode = 3;
                    }

                }else
                {
                    movement.Enabled = false;
                }
            }
            else
            {
                if (MoveMode == 1)
                {
                    DrawThing(-5);
                }
                else if(MoveMode == 0)
                {
                    DrawThing(5);
                }
                else if (MoveMode == 2)
                {
                    PLaySound(3);
                    //pew pew code
                }
                else if (MoveMode == 3)
                {
                    if (ControlVar == 200)
                    {
                        PLaySound(2);
                    }
                }
                ControlVar++;
            }
        }

        void DrawThing(int Change)
        {
            if (Change == 0)
            {
                RightVar = 92;
                LeftVar = 2;
            }else{
                RightVar += Change;
                LeftVar -= Change;
            }

            int width = 223, height = 536, x = 100, y = 100;
            System.Drawing.Image image = new Bitmap(width + 200, height + 200);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(Portal_Turret.Properties.Resources.Portal_Turret_Right, new Rectangle(RightVar + x, 38 + y, 122, 307));
                graphics.DrawImage(Portal_Turret.Properties.Resources.Portal_Turret_Left, new Rectangle(LeftVar + x, 38 + y, 122, 307));
                graphics.DrawImage(Portal_Turret.Properties.Resources.Turret_Body__2_, new Rectangle(0 + x, 0 + y, 223, 536));
            }

            tBody.Image = image;
        }

        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> NamePersons = new List<string>();
        int GoneTimer;

        private void camera_Tick(object sender, EventArgs e)
        {
            //label3.Text = "0";
            //label4.Text = "";
            NamePersons.Add("");


            //Get the current frame form capture device
            currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

            //Convert it to Grayscale
            gray = currentFrame.Convert<Gray, Byte>();

            //Face Detector
            MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(
          face,
          1.2,
          10,
          Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
          new Size(20, 20));

            //Action for each element detected
            foreach (MCvAvgComp f in facesDetected[0])
            {
                t = t + 1;
                result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                //draw the face detected in the 0th (gray) channel with blue color
                currentFrame.Draw(f.rect, new Bgr(Color.Red), 2);


                if (trainingImages.ToArray().Length != 0)
                {
                    //TermCriteria for face recognition with numbers of trained images like maxIteration
                    MCvTermCriteria termCrit = new MCvTermCriteria(ContTrain, 0.001);

                    //Eigen face recognizer
                    EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                       trainingImages.ToArray(),
                       labels.ToArray(),
                       3000,
                       ref termCrit);

                    name = recognizer.Recognize(result);

                    //Draw the label for each face detected and recognized
                    //currentFrame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.LightGreen));

                }

                NamePersons[t - 1] = name;
                NamePersons.Add("");


                //Set the number of faces detected on the scene
                // label3.Text = facesDetected[0].Length.ToString();

                /*
                //Set the region of interest on the faces

                gray.ROI = f.rect;
                MCvAvgComp[][] eyesDetected = gray.DetectHaarCascade(
                   eye,
                   1.1,
                   10,
                   Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                   new Size(20, 20));
                gray.ROI = Rectangle.Empty;

                foreach (MCvAvgComp ey in eyesDetected[0])
                {
                    Rectangle eyeRect = ey.rect;
                    eyeRect.Offset(f.rect.X, f.rect.Y);
                    currentFrame.Draw(eyeRect, new Bgr(Color.Blue), 2);
                }
                 */

            }
            t = 0;

            //Names concatenation of persons recognized

            if (Convert.ToInt16(facesDetected[0].Length.ToString()) != 0)
            {
                if (MoveMode == 1)
                {
                    OpenTurret();
                    Search = false;
                }

                if (GoneTimer == 15)
                {
                    PLaySound(3);
                }
                else
                {
                    GoneTimer++;
                }
            }
            else
            {
                if ( GoneTimer != 0)
                {
                    Search = true;
                    GoneTimer = 0;
                }
            }

           // MessageBox.Show(facesDetected[0].Length.ToString());

        }

        int ContTrain, NumLabels, t;
        string name;

        void CameraTest()
        {
            //Load haarcascades for face detection
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            //eye = new HaarCascade("haarcascade_eye.xml");
            try
            {
                //Load of previus trainned faces and labels for each image
                string Labelsinfo = File.ReadAllText(Application.StartupPath + "/TrainedFaces/TrainedLabels.txt");
                string[] Labels = Labelsinfo.Split('%');
                NumLabels = Convert.ToInt16(Labels[0]);
                ContTrain = NumLabels;
                string LoadFaces;

                for (int tf = 1; tf < NumLabels + 1; tf++)
                {
                    LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/TrainedFaces/" + LoadFaces));
                    labels.Add(Labels[tf]);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            //Initialize the capture device
            grabber = new Capture();
            grabber.QueryFrame();
            //Initialize the FrameGraber event

            camera.Enabled = true;
        }

    }

}