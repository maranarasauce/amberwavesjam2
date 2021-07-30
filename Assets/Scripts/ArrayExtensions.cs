using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ArrayExtensions
    {
        public static T RandomEntry<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        } 
    }
