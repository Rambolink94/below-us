using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BelowUs.Utility
{
    public static class HelperFunctions
    {
        public static float RoundFloatNearestHalfOrWhole(float numToRound)
        {
            return (float)Math.Round(numToRound * 2, MidpointRounding.AwayFromZero) / 2;
        }

        public static float RoundFloatNearestWhole(float numToRound)
        {
            return (float)Math.Round(numToRound);
        }

        public static Vector3 RoundVector3NearestHalfOrWhole(Vector3 vectorToRound)
        {
            float x = RoundFloatNearestHalfOrWhole(vectorToRound.x);
            float y = RoundFloatNearestHalfOrWhole(vectorToRound.y);
            float z = RoundFloatNearestHalfOrWhole(vectorToRound.z);

            return new Vector3(x, y, z);
        }

        public static Vector3 RoundVector3NearestWhole(Vector3 vectorToRound)
        {
            float x = RoundFloatNearestWhole(vectorToRound.x);
            float y = RoundFloatNearestWhole(vectorToRound.y);
            float z = RoundFloatNearestWhole(vectorToRound.z);

            return new Vector3(x, y, z);
        }
    }
}
