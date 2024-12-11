using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verlet;

namespace DefaultNamespace
{
    public static class SeamMaker
    {
        public static int[] EqualizeSeamLength(int length1, int length2)
        {
            var (max, min) = length1 >= length2 
                ? (length1, length2) 
                : (length2, length1);
            int[] results = new int[max];
            float lerpStep = (float)1 / max;

            for (int i = 0; i < max; i++)
            {
                results[i] = (Mathf.FloorToInt(Mathf.Lerp(0, min, lerpStep*i)));
            }
            return results;
        }

        public static void ConnectSeams(List<VerletNode> seam1, List<VerletNode> seam2)
        {
            var (max, min) = seam1.Count >= seam2.Count 
                ? (seam1, seam2) 
                : (seam2, seam1);
            var distribution = EqualizeSeamLength(seam1.Count, seam2.Count);

            for (int i = 0; i < distribution.Length; i++)
            {
                MakeSeam(max[i], min[distribution[i]]);
            }
        }

        private static void MakeSeam(VerletNode one, VerletNode two)
        {
            VerletEdge.ConnectNodes(one, two, 0.1f, VerletEdge.EdgeType.Seam);
            one.isSeam = true;
            two.isSeam = true;
            one.SetMarbleRadius(new Vector2(0,0));
            two.SetMarbleRadius(new Vector2(0,0));
        }

    }
}