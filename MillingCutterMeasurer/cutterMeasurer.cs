using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV;


namespace MillingCutterMeasurer
{
     public class iPoint
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public iPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class cutterMeasurer
    {

        public double calibration;

        public string message { get; private set; }
        public Bitmap anotatedImage { get; private set; }
        public Bitmap edge { get; private set; }

        public int numLU_X, numLU_Y, numWidth, numHeight;
        public int numLS, numLB, numRS, numRB;
        public double handleStart, handleLength;
        public int tailPosition,headPosition,totalPixel;
        public double totalLength;

        public cutterMeasurer(System.Drawing.Bitmap image, int lux=450, int luy=900, int w=200, int h=300, int ls=240, int lb=10, int rs=850, int rb=210, double calibFactor = 19.1666666)
        {
            this.calibration = calibFactor;
            this.message = "";

            this.numLU_X = lux;
            this.numLU_Y = luy;
            this.numWidth = w;
            this.numHeight = h;
            this.numLS = ls;
            this.numLB = lb;
            this.numRS = rs;
            this.numRB = rs;


            Image<Bgr, Byte> img = new Image<Bgr, byte>(image);

            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img, uimage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);


            //canny edge detection
            double cannyThreshold = 80.0;
            double cannyThresholdLinking = 40.0;
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(uimage, cannyEdges, cannyThreshold, cannyThresholdLinking);

            Image<Gray, Byte> cannyImg = cannyEdges.ToImage<Gray, Byte>();
            //doMeasure(cannyImg);
            iPoint[] initialPoint = pixelsInBox(cannyImg, numLU_X, numLU_Y, numWidth, numHeight);

            double rsqrt, sloop, intercept;
            if (!this.LinearRegression(initialPoint, out rsqrt, out intercept, out sloop))
            {
                this.message = "無待測物";
                return;
            }
            //1次校正
            Image<Bgr, Byte> cannyColor = cannyEdges.ToImage<Bgr, Byte>();

            if (Math.Abs(sloop) > 0.01)
            {
                cannyColor = cannyEdges.ToImage<Bgr, Byte>().Rotate(-1 * (Math.Atan(sloop) / (Math.PI / 180)), new Bgr(), false);
                cannyImg = new Image<Gray, Byte>(cannyColor.Bitmap);
                initialPoint = pixelsInBox(cannyImg, numLU_X, numLU_Y, numWidth, numHeight);
                this.LinearRegression(initialPoint, out rsqrt, out intercept, out sloop);
            }

            if (Math.Abs(sloop) > 0.02)
            {
                message = "影像過歪";
                return;
            }

            //取柄徑
            double handdle = measureDiameter(cannyImg, numLU_X, numWidth, numHeight / 2, intercept, sloop);


            //取尾端
            int xs = numLS;
            int xTail = findLeft(cannyImg, xs, numLB, (int)(intercept + (xs * sloop)), (int)handdle);

            //取前端
            xs = numRS;
            int xHead = findRight(cannyImg, xs, numRB, (int)(intercept + (xs * sloop)), (int)handdle);

            int xHandelStart = 0;

            if (xTail < 0)
            {
                message = "柄端檢測失敗";
                return;
            }
            else if (xHead < 0)
            {
                message = "刃端檢測失敗";
                return;
            }
            else
            {
                xHandelStart = findCutterStart(cannyImg, xs, xHead, (int)(intercept + (xs * sloop)), (int)handdle, 15, 0.1);
                //xHandelStart = findCutterStart(cannyImg, xs, xHead, (int)(intercept + (xs * sloop)), 3, 1);
                if (xHandelStart < 0)
                {
                    message = "測定柄長失敗";
                    return;
                }
                else
                {
                    handleLength = xHandelStart + xs - xTail;
                    handleStart = xHandelStart;

                }
            }

            tailPosition = xTail;
            headPosition = xHead;
            int intLen = (xHead - xTail);
            totalPixel = intLen;
            totalLength = intLen * this.calibration;

            this.edge = cannyColor.Bitmap;

            Graphics g = Graphics.FromImage(image);
            Pen redP = new Pen(Color.Red);
            Pen blueP = new Pen(Color.Red);

            int lineH = (int)(handdle / 2) + 5;

            int high = image.Height;
            int lbpos = numLB;
            int rbpos = numRB;
            int lspos = numLS;
            int rspos = numRS;

            g.DrawLine(redP, lbpos, 0, lbpos, high);
            g.DrawLine(redP, rbpos, 0, rbpos, high);
            g.DrawLine(redP, lspos, 0, lspos, high);
            g.DrawLine(redP, rspos, 0, rspos, high);


            //center line
            g.DrawLine(redP, 0, (float)intercept, (float)rbpos, (float)(intercept + (rbpos * sloop)));

            //tail line
            this.drawHorLine(g, blueP, xTail, intercept, sloop, lineH);
            this.drawHorLine(g, blueP, xHead, intercept, sloop, lineH);
            this.drawHorLine(g, blueP, xHandelStart + xs, intercept, sloop, lineH);
        }

        private iPoint[] pixelsInBox(Image<Gray, Byte> img, int xp, int yp, int w, int h)
        {
            List<iPoint> rtn = new List<iPoint>();
            for (int x = xp; x < xp + w; ++x)
            {
                for (int y = yp; y < yp + h; ++y)
                {
                    if (img.Data[y, x, 0] > 0) rtn.Add(new iPoint(x, y));
                }
            }
            return rtn.ToArray();
        }

        private void drawHorLine(Graphics g, Pen p, int xAxis, double intercept, double sloop, int lineH)
        {
            g.DrawLine(p, (float)xAxis, (float)(xAxis * sloop) + (float)(intercept - lineH), (float)xAxis, (float)((xAxis * sloop) + intercept + lineH));

        }

        private double getStandardDeviation(List<double> doubleList)
        {
            double average = doubleList.Average();
            double sumOfDerivation = 0;
            foreach (double value in doubleList)
            {
                sumOfDerivation += (value) * (value);
            }
            double sumOfDerivationAverage = sumOfDerivation / doubleList.Count;
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }

        private int findCutterStart(Image<Gray, Byte> img, int xStart, int xEnd, int yCenter, int seekHigh,int wLen=5, double thr=0.4)
        {
            if (xStart >= xEnd) throw new Exception("findCutterStart : error initial index");
            List<double> diameters = new List<double>();
            int h = seekHigh;

            int errCnt=0;

			xEnd -= 25;  //刃尖不是平的 會所以後退一段距離
            for (int x = xStart; x < xEnd; ++x)
            {
                int uUp = -1;
                int uDown = -1;

                //
                for (int yi = 0; yi < seekHigh; ++yi)
                {
                    if (img.Data[yCenter-yi, x, 0] > 0)
                    {
                        uUp = yi;
                        break;
                    }
                }

                for (int yi = 0; yi < seekHigh; ++yi)
                {
                    if (img.Data[yCenter+yi, x, 0] > 0)
                    {
                        uDown = yi;
                        break;
                    }
                }

                if (uUp < 0 || uDown < 0)
                {
                    errCnt++;
                    if (errCnt > 3)
                    {
                        this.message = "柄長偵測失敗";
                        return -1;
                    }
                }
                else
                {
                    diameters.Add(uDown + uUp + 1);
                }
            }

            double[] vals=diameters.ToArray();
            List<double> sloops = new List<double>();
            double[] temp=new double[wLen];
            for (int st = 0; (st+wLen)<vals.Length; ++st)
            {
                Array.Copy(vals, st, temp, 0, wLen);
                sloops.Add(arraySloop(temp));
            }

            double value = 0;
            for (int j = 0; j < (wLen - 1); ++j) value += sloops[j];

            for (int i = wLen - 1; i < sloops.Count; ++i)
            {
                value += sloops[i];
                if (Math.Abs(value /wLen) >= thr) 
                    return i;
                value -= sloops[i + 1 - wLen]; ;

            }

            return -1;
        }

        private double arraySloop(double[] arr)
        {
            if (arr == null || arr.Length < 1) throw new Exception("Empty array");
            double sum = 0;
            for (int i = 1; i < arr.Length; ++i) sum += (arr[i] - arr[i - 1]);
            return sum / (arr.Length - 1);
        }

        private int findLeft(Image<Gray, Byte> img, int xStart, int xEnd, int yCenter, int seekHigh)
        {
            if (xStart <= xEnd) throw new Exception("error initial index");
            int rtn=-1;

            int h = (int) (seekHigh/4);
            for (int x = xStart; x > xEnd; --x)
            {
                if (rtn < 0)
                {
                    for (int yi = 1; yi < h; ++yi)
                    {
                        if (img.Data[yCenter - yi, x, 0] > 0 || img.Data[yCenter + yi,x , 0] > 0)
                        {
                            rtn = x;
                            h = (int)(seekHigh / 2)+5;
                            break;
                        }
                    }
                }
                else
                {
                    bool edge = false;
                    for (int yi = 1; yi < h; ++yi)
                    {
                        if (img.Data[ yCenter - yi, x,0] > 0 || img.Data[ yCenter + yi,x, 0] > 0)
                        {
                            edge = true;
                            break;
                        }
                    }
                    if (edge == false)
                    {
                        return x+1;
                    }
                }
                
            }
            return -1;
        }

        private int findRight(Image<Gray, Byte> img, int xStart, int xEnd, int yCenter, int seekHigh)
        {
            if (xStart >= xEnd) throw new Exception("error initial index");
            int rtn = -1;

            int h = (int)(seekHigh / 4);
            for (int x = xStart; x < xEnd; ++x)
            {
                if (rtn < 0)
                {
                    for (int yi = 1; yi < h; ++yi)
                    {
                        if (img.Data[yCenter - yi, x, 0] > 0 || img.Data[yCenter + yi, x, 0] > 0)
                        {
                            rtn = x;
                            h = (int)(seekHigh / 2) + 5;
                            break;
                        }
                    }
                }
                else
                {
                    bool edge = false;
                    for (int yi = 1; yi < h; ++yi)
                    {
                        if (img.Data[yCenter - yi, x, 0] > 0 || img.Data[yCenter + yi, x, 0] > 0)
                        {
                            edge = true;
                            break;
                        }
                    }
                    if (edge == false)
                    {
                        return x;
                    }
                }

            }
            return -1;
        }

        private double measureDiameter(Image<Gray, Byte> img, int xStart, int xLength, int yHigh, double intercept, double sloop)
        {

            int[] up = new int[xLength];
            int[] down = new int[xLength];
            for (int xi = 0; xi < xLength; ++xi)
            {
                int x = xi + xStart;
                int ySt = (int)(intercept + (sloop * x));

                //up
                for (int y = 1; y < yHigh; ++y)
                {
                    if (img.Data[ySt-y,x , 0] > 0)
                    {
                        up[xi] = y;
                        break;
                    }
                }

                //down
                for (int y = 1; y < yHigh; ++y)
                {
                    if (img.Data[ySt + y,x,  0] > 0)
                    {
                        down[xi] = y;
                        break;
                    }
                }

                
            }

            double rtn = up.Average() + down.Average();
            return rtn;
        }
        
        private void doMeasure(Image<Gray,Byte> img) {
            iPoint[] initialPoint = pixelsInBox(img, numLU_X, numLU_Y, numWidth, numHeight);

            double rsqrt;
            double sloop;
            double intercept;

            this.LinearRegression(initialPoint, out rsqrt, out intercept, out sloop);
        }

        public bool LinearRegression(iPoint[] points,
                                        out double rsquared, out double yintercept,
                                        out double slope)
        {
            if (points.Length < 1)
            {
                rsquared = 0;
                yintercept = 0;
                slope = 0;
                return false;
            }
            double[] xVals = new double[points.Length];
            double[] yVals = new double[points.Length];

            for (int i = 0; i < points.Length; ++i)
            {
                xVals[i] = (double) points[i].X;
                yVals[i] = (double) points[i].Y;
            }

            LinearRegression(xVals,yVals,out rsquared,out yintercept, out slope);
            return true;
        }

        public void LinearRegression(double[] xVals, double[] yVals,
                                        out double rsquared, out double yintercept,
                                        out double slope)
        {
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = xVals.Length;

            for (int ctr = 0; ctr < xVals.Length; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }
    }
}
