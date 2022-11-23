using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MillingCutterMeasurer
{
    public class statistics
    {

        static public bool LinearRegression(System.Drawing.Point[] points,
                                       out double rsquared, out double yintercept,
                                       out double slope, bool xyTrans=false)
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
                if(xyTrans)
                {
                    xVals[i] = (double)points[i].Y;
                    yVals[i] = (double)points[i].X;
                }
                else
                {
                    xVals[i] = (double)points[i].X;
                    yVals[i] = (double)points[i].Y;
                }
            }

            LinearRegression(xVals, yVals, out rsquared, out yintercept, out slope);
            return true;
        }

        static public void LinearRegression(double[] xVals, double[] yVals,
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

        static public double[] smoothByFlank(int[] array, int start, int end, int flank)
        {
            double[] rtn = new double[array.Length];
            //do smooth
            for (int i = start; i < end; ++i)
                rtn[i] = smoothByFlankSub(array, start, end, flank, i);

			double[] tRtn = new double[end - start];
			Array.Copy(array, start, tRtn, 0, tRtn.Length); 

            return rtn;
        }

		
        static private double smoothByFlankSub(int[] array, int start, int end, int flank, int index)
        {
            if (index < start || index > end) return array[index];
			int cnt=0;
			double sum=0;
			if(array[index]>0)
			{
				++cnt;
				sum = array[index];
			}

			int i = 0;
			for (i = 1; i <= flank; ++i)
			{
				if ((index + i) < end && array[index + i] > 0)
				{
					sum += array[index + i];
					++cnt;
				}
				if ((index - i) >= start && array[index - i] > 0)
				{
					sum += array[index - i];
					++cnt;
				}
			}

            return sum / cnt;
        }

        static public double[] sloopByFlank(double[] array, int start, int end, int flank)
        {
            double[] rtn = new double[array.Length];
            for (int i = 0; i < array.Length; ++i)
                rtn[i] = 0;
            for (int i = start; i <= end; ++i)
                rtn[i]=smoothByFlankSub(array, start, end, flank, i);

            return rtn;
        }

		static public double[] smoothByFlank(double[] array, int start, int end, int flank)
		{
			double[] rtn = new double[array.Length];
			//do smooth
			for (int i = start; i < end; ++i)
				rtn[i] = smoothByFlankDouble(array, start, end, flank, i);

			return rtn;
		}
		static public double smoothByFlankDouble(double[] array, int start, int end, int flank, int index)
		{
			int tStart = index - flank;
			if (tStart < start)
				tStart = start;
			int tEnd = index + flank;
			if (tEnd >= end)
				tEnd = end;

			if (tStart > end)
				return double.NaN;

			double sum = 0;
			double cnt = 0;
			for(int i=tStart; i<end; ++i)
			{
				if(!double.IsNaN(array[i]))
				{
					sum += array[i];
					cnt++;
				}
			}

			return (cnt > 0) ? sum / cnt : double.NaN;
		}
		static public double[] stageSloops(double[] array, int start, int end, int flank)
		{
			double[] rtn = new double[array.Length];
			for (int i = 0; i < array.Length; ++i)
				rtn[i] = stageSloopsSub(array, start, end, flank, i) * 10;
			return rtn;
		}
		static private double stageSloopsSub(double[] array, int start, int end, int flank, int index)
		{
			int subStart = start + flank;
			int subEnd = end - flank;
			if (index < subStart || index >= subEnd)
				return double.NaN;

			try
			{

				double[] left = new double[flank];
				Array.Copy(array, index - flank, left, 0, flank);
				double[] right = new double[flank];
				Array.Copy(array, index, right, 0, flank);

				return (left.Average() / right.Average()) - 1;
			}
			catch (Exception ex)
			{
				return Double.NaN;
			}

		}


		static private double smoothByFlankSub(double[] array, int start, int end, int flank, int idx)
        {
			if (idx < start || idx > end)
				return double.NaN;

			int lb = idx - flank;
			if (lb < start) lb = start;
			int rb = idx + flank;
			if (rb >= end) rb = end-1;

			if (lb >= rb) return 0;

			double sum = 0;
			int cnt = 0;
			for (int i = lb + 1; i <= rb; ++i)
			{
				sum += (array[i] - array[i - 1]);
				++cnt;
			}

			return sum / cnt;

        }
    }

    
}
