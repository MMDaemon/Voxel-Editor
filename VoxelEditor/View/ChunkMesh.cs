﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Zenseless.Geometry;
using VoxelUtils;
using VoxelUtils.Shared;

namespace VoxelEditor.View
{
    internal class ChunkMesh : VoxelMesh
    {
        private readonly float sqrtHalf;

        private static readonly int[] FacesLookup ={
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 8, 3, 1, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        9, 2, 11, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        2, 8, 3, 2, 11, 8, 11, 9, 8, -1, -1, -1, -1, -1, -1,
        3, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 10, 2, 8, 10, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 9, 0, 2, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 10, 2, 1, 9, 10, 9, 8, 10, -1, -1, -1, -1, -1, -1,
        3, 11, 1, 10, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 11, 1, 0, 8, 11, 8, 10, 11, -1, -1, -1, -1, -1, -1,
        3, 9, 0, 3, 10, 9, 10, 11, 9, -1, -1, -1, -1, -1, -1,
        9, 8, 11, 11, 8, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1,
        1, 2, 11, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        3, 4, 7, 3, 0, 4, 1, 2, 11, -1, -1, -1, -1, -1, -1,
        9, 2, 11, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1,
        2, 11, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1,
        8, 4, 7, 3, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        10, 4, 7, 10, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1,
        9, 0, 1, 8, 4, 7, 2, 3, 10, -1, -1, -1, -1, -1, -1,
        4, 7, 10, 9, 4, 10, 9, 10, 2, 9, 2, 1, -1, -1, -1,
        3, 11, 1, 3, 10, 11, 7, 8, 4, -1, -1, -1, -1, -1, -1,
        1, 10, 11, 1, 4, 10, 1, 0, 4, 7, 10, 4, -1, -1, -1,
        4, 7, 8, 9, 0, 10, 9, 10, 11, 10, 0, 3, -1, -1, -1,
        4, 7, 10, 4, 10, 9, 9, 10, 11, -1, -1, -1, -1, -1, -1,
        9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1,
        1, 2, 11, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        3, 0, 8, 1, 2, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1,
        5, 2, 11, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1,
        2, 11, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1,
        9, 5, 4, 2, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 10, 2, 0, 8, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1,
        0, 5, 4, 0, 1, 5, 2, 3, 10, -1, -1, -1, -1, -1, -1,
        2, 1, 5, 2, 5, 8, 2, 8, 10, 4, 8, 5, -1, -1, -1,
        11, 3, 10, 11, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1,
        4, 9, 5, 0, 8, 1, 8, 11, 1, 8, 10, 11, -1, -1, -1,
        5, 4, 0, 5, 0, 10, 5, 10, 11, 10, 0, 3, -1, -1, -1,
        5, 4, 8, 5, 8, 11, 11, 8, 10, -1, -1, -1, -1, -1, -1,
        9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1,
        0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1,
        1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        9, 7, 8, 9, 5, 7, 11, 1, 2, -1, -1, -1, -1, -1, -1,
        11, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1,
        8, 0, 2, 8, 2, 5, 8, 5, 7, 11, 5, 2, -1, -1, -1,
        2, 11, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1,
        7, 9, 5, 7, 8, 9, 3, 10, 2, -1, -1, -1, -1, -1, -1,
        9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 10, -1, -1, -1,
        2, 3, 10, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1,
        10, 2, 1, 10, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1,
        9, 5, 8, 8, 5, 7, 11, 1, 3, 11, 3, 10, -1, -1, -1,
        5, 7, 0, 5, 0, 9, 7, 10, 0, 1, 0, 11, 10, 11, 0,
        10, 11, 0, 10, 0, 3, 11, 5, 0, 8, 0, 7, 5, 7, 0,
        10, 11, 5, 7, 10, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        11, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 8, 3, 5, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        9, 0, 1, 5, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 8, 3, 1, 9, 8, 5, 11, 6, -1, -1, -1, -1, -1, -1,
        1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1,
        9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1,
        5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1,
        2, 3, 10, 11, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        10, 0, 8, 10, 2, 0, 11, 6, 5, -1, -1, -1, -1, -1, -1,
        0, 1, 9, 2, 3, 10, 5, 11, 6, -1, -1, -1, -1, -1, -1,
        5, 11, 6, 1, 9, 2, 9, 10, 2, 9, 8, 10, -1, -1, -1,
        6, 3, 10, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1,
        0, 8, 10, 0, 10, 5, 0, 5, 1, 5, 10, 6, -1, -1, -1,
        3, 10, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1,
        6, 5, 9, 6, 9, 10, 10, 9, 8, -1, -1, -1, -1, -1, -1,
        5, 11, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 3, 0, 4, 7, 3, 6, 5, 11, -1, -1, -1, -1, -1, -1,
        1, 9, 0, 5, 11, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1,
        11, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1,
        6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1,
        1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1,
        8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1,
        7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9,
        3, 10, 2, 7, 8, 4, 11, 6, 5, -1, -1, -1, -1, -1, -1,
        5, 11, 6, 4, 7, 2, 4, 2, 0, 2, 7, 10, -1, -1, -1,
        0, 1, 9, 4, 7, 8, 2, 3, 10, 5, 11, 6, -1, -1, -1,
        9, 2, 1, 9, 10, 2, 9, 4, 10, 7, 10, 4, 5, 11, 6,
        8, 4, 7, 3, 10, 5, 3, 5, 1, 5, 10, 6, -1, -1, -1,
        5, 1, 10, 5, 10, 6, 1, 0, 10, 7, 10, 4, 0, 4, 10,
        0, 5, 9, 0, 6, 5, 0, 3, 6, 10, 6, 3, 8, 4, 7,
        6, 5, 9, 6, 9, 10, 4, 7, 9, 7, 10, 9, -1, -1, -1,
        11, 4, 9, 6, 4, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 11, 6, 4, 9, 11, 0, 8, 3, -1, -1, -1, -1, -1, -1,
        11, 0, 1, 11, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1,
        8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 11, -1, -1, -1,
        1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1,
        3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1,
        0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1,
        11, 4, 9, 11, 6, 4, 10, 2, 3, -1, -1, -1, -1, -1, -1,
        0, 8, 2, 2, 8, 10, 4, 9, 11, 4, 11, 6, -1, -1, -1,
        3, 10, 2, 0, 1, 6, 0, 6, 4, 6, 1, 11, -1, -1, -1,
        6, 4, 1, 6, 1, 11, 4, 8, 1, 2, 1, 10, 8, 10, 1,
        9, 6, 4, 9, 3, 6, 9, 1, 3, 10, 6, 3, -1, -1, -1,
        8, 10, 1, 8, 1, 0, 10, 6, 1, 9, 1, 4, 6, 4, 1,
        3, 10, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1,
        6, 4, 8, 10, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        7, 11, 6, 7, 8, 11, 8, 9, 11, -1, -1, -1, -1, -1, -1,
        0, 7, 3, 0, 11, 7, 0, 9, 11, 6, 7, 11, -1, -1, -1,
        11, 6, 7, 1, 11, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1,
        11, 6, 7, 11, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1,
        1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1,
        2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9,
        7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1,
        7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        2, 3, 10, 11, 6, 8, 11, 8, 9, 8, 6, 7, -1, -1, -1,
        2, 0, 7, 2, 7, 10, 0, 9, 7, 6, 7, 11, 9, 11, 7,
        1, 8, 0, 1, 7, 8, 1, 11, 7, 6, 7, 11, 2, 3, 10,
        10, 2, 1, 10, 1, 7, 11, 6, 1, 6, 7, 1, -1, -1, -1,
        8, 9, 6, 8, 6, 7, 9, 1, 6, 10, 6, 3, 1, 3, 6,
        0, 9, 1, 10, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        7, 8, 0, 7, 0, 6, 3, 10, 0, 10, 6, 0, -1, -1, -1,
        7, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        7, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        3, 0, 8, 10, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 1, 9, 10, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        8, 1, 9, 8, 3, 1, 10, 7, 6, -1, -1, -1, -1, -1, -1,
        11, 1, 2, 6, 10, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 2, 11, 3, 0, 8, 6, 10, 7, -1, -1, -1, -1, -1, -1,
        2, 9, 0, 2, 11, 9, 6, 10, 7, -1, -1, -1, -1, -1, -1,
        6, 10, 7, 2, 11, 3, 11, 8, 3, 11, 9, 8, -1, -1, -1,
        7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1,
        2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1,
        1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1,
        11, 7, 6, 11, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1,
        11, 7, 6, 1, 7, 11, 1, 8, 7, 1, 0, 8, -1, -1, -1,
        0, 3, 7, 0, 7, 11, 0, 11, 9, 6, 11, 7, -1, -1, -1,
        7, 6, 11, 7, 11, 8, 8, 11, 9, -1, -1, -1, -1, -1, -1,
        6, 8, 4, 10, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        3, 6, 10, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1,
        8, 6, 10, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1,
        9, 4, 6, 9, 6, 3, 9, 3, 1, 10, 3, 6, -1, -1, -1,
        6, 8, 4, 6, 10, 8, 2, 11, 1, -1, -1, -1, -1, -1, -1,
        1, 2, 11, 3, 0, 10, 0, 6, 10, 0, 4, 6, -1, -1, -1,
        4, 10, 8, 4, 6, 10, 0, 2, 9, 2, 11, 9, -1, -1, -1,
        11, 9, 3, 11, 3, 2, 9, 4, 3, 10, 3, 6, 4, 6, 3,
        8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1,
        0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1,
        1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1,
        8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 11, 1, -1, -1, -1,
        11, 1, 0, 11, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1,
        4, 6, 3, 4, 3, 8, 6, 11, 3, 0, 3, 9, 11, 9, 3,
        11, 9, 4, 6, 11, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 9, 5, 7, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 8, 3, 4, 9, 5, 10, 7, 6, -1, -1, -1, -1, -1, -1,
        5, 0, 1, 5, 4, 0, 7, 6, 10, -1, -1, -1, -1, -1, -1,
        10, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1,
        9, 5, 4, 11, 1, 2, 7, 6, 10, -1, -1, -1, -1, -1, -1,
        6, 10, 7, 1, 2, 11, 0, 8, 3, 4, 9, 5, -1, -1, -1,
        7, 6, 10, 5, 4, 11, 4, 2, 11, 4, 0, 2, -1, -1, -1,
        3, 4, 8, 3, 5, 4, 3, 2, 5, 11, 5, 2, 10, 7, 6,
        7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1,
        9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1,
        3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1,
        6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8,
        9, 5, 4, 11, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1,
        1, 6, 11, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4,
        4, 0, 11, 4, 11, 5, 0, 3, 11, 6, 11, 7, 3, 7, 11,
        7, 6, 11, 7, 11, 8, 5, 4, 11, 4, 8, 11, -1, -1, -1,
        6, 9, 5, 6, 10, 9, 10, 8, 9, -1, -1, -1, -1, -1, -1,
        3, 6, 10, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1,
        0, 10, 8, 0, 5, 10, 0, 1, 5, 5, 6, 10, -1, -1, -1,
        6, 10, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1,
        1, 2, 11, 9, 5, 10, 9, 10, 8, 10, 5, 6, -1, -1, -1,
        0, 10, 3, 0, 6, 10, 0, 9, 6, 5, 6, 9, 1, 2, 11,
        10, 8, 5, 10, 5, 6, 8, 0, 5, 11, 5, 2, 0, 2, 5,
        6, 10, 3, 6, 3, 5, 2, 11, 3, 11, 5, 3, -1, -1, -1,
        5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1,
        9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1,
        1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8,
        1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 3, 6, 1, 6, 11, 3, 8, 6, 5, 6, 9, 8, 9, 6,
        11, 1, 0, 11, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1,
        0, 3, 8, 5, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        11, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        10, 5, 11, 7, 5, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        10, 5, 11, 10, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1,
        5, 10, 7, 5, 11, 10, 1, 9, 0, -1, -1, -1, -1, -1, -1,
        11, 7, 5, 11, 10, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1,
        10, 1, 2, 10, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1,
        0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 10, -1, -1, -1,
        9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 10, 7, -1, -1, -1,
        7, 5, 2, 7, 2, 10, 5, 9, 2, 3, 2, 8, 9, 8, 2,
        2, 5, 11, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1,
        8, 2, 0, 8, 5, 2, 8, 7, 5, 11, 2, 5, -1, -1, -1,
        9, 0, 1, 5, 11, 3, 5, 3, 7, 3, 11, 2, -1, -1, -1,
        9, 8, 2, 9, 2, 1, 8, 7, 2, 11, 2, 5, 7, 5, 2,
        1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1,
        9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1,
        9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        5, 8, 4, 5, 11, 8, 11, 10, 8, -1, -1, -1, -1, -1, -1,
        5, 0, 4, 5, 10, 0, 5, 11, 10, 10, 3, 0, -1, -1, -1,
        0, 1, 9, 8, 4, 11, 8, 11, 10, 11, 4, 5, -1, -1, -1,
        11, 10, 4, 11, 4, 5, 10, 3, 4, 9, 4, 1, 3, 1, 4,
        2, 5, 1, 2, 8, 5, 2, 10, 8, 4, 5, 8, -1, -1, -1,
        0, 4, 10, 0, 10, 3, 4, 5, 10, 2, 10, 1, 5, 1, 10,
        0, 2, 5, 0, 5, 9, 2, 10, 5, 4, 5, 8, 10, 8, 5,
        9, 4, 5, 2, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        2, 5, 11, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1,
        5, 11, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1,
        3, 11, 2, 3, 5, 11, 3, 8, 5, 4, 5, 8, 0, 1, 9,
        5, 11, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1,
        8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1,
        0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1,
        9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 10, 7, 4, 9, 10, 9, 11, 10, -1, -1, -1, -1, -1, -1,
        0, 8, 3, 4, 9, 7, 9, 10, 7, 9, 11, 10, -1, -1, -1,
        1, 11, 10, 1, 10, 4, 1, 4, 0, 7, 4, 10, -1, -1, -1,
        3, 1, 4, 3, 4, 8, 1, 11, 4, 7, 4, 10, 11, 10, 4,
        4, 10, 7, 9, 10, 4, 9, 2, 10, 9, 1, 2, -1, -1, -1,
        9, 7, 4, 9, 10, 7, 9, 1, 10, 2, 10, 1, 0, 8, 3,
        10, 7, 4, 10, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1,
        10, 7, 4, 10, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1,
        2, 9, 11, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1,
        9, 11, 7, 9, 7, 4, 11, 2, 7, 8, 7, 0, 2, 0, 7,
        3, 7, 11, 3, 11, 2, 7, 4, 11, 1, 11, 0, 4, 0, 11,
        1, 11, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1,
        4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1,
        4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        9, 11, 8, 11, 10, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        3, 0, 9, 3, 9, 10, 10, 9, 11, -1, -1, -1, -1, -1, -1,
        0, 1, 11, 0, 11, 8, 8, 11, 10, -1, -1, -1, -1, -1, -1,
        3, 1, 11, 10, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 2, 10, 1, 10, 9, 9, 10, 8, -1, -1, -1, -1, -1, -1,
        3, 0, 9, 3, 9, 10, 1, 2, 9, 2, 10, 9, -1, -1, -1,
        0, 2, 10, 8, 0, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        3, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        2, 3, 8, 2, 8, 11, 11, 8, 9, -1, -1, -1, -1, -1, -1,
        9, 11, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        2, 3, 8, 2, 8, 11, 0, 1, 8, 1, 11, 8, -1, -1, -1,
        1, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
    };

        public ChunkMesh(Chunk chunk, int sizeX = Constant.ChunkSizeX, int sizeY = Constant.ChunkSizeY, int sizeZ = Constant.ChunkSizeZ)
        {
            //sqrtHalf = (float)Math.Sqrt(0.5);
            sqrtHalf = (float)(Math.Pow(3 / Math.Sqrt(2), 1.0 / 3.0) / Math.Sqrt(2));

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        AddVoxelMesh(chunk, new Vector3I(x, y, z));
                    }
                }
            }
        }

        public ChunkMesh(Chunk chunk, Vector3I size) : this(chunk, size.X, size.Y, size.Z)
        {
        }

        private void AddVoxelMesh(Chunk chunk, Vector3I pos)
        {

            List<Voxel> neededVoxels = CalculateNeededVoxels(chunk, pos);
            List<int> vertexNumbers = GetVertexNumbersFromLookup(neededVoxels);

            if (vertexNumbers.Count > 0)
            {
                VoxelMesh voxelMesh = new VoxelMesh();

                List<float> internalDistances = CalculateInternalDistances(neededVoxels);
                List<Vector3> internalPositions = CalculateInternalPositions(internalDistances);

                List<float> materialAmounts = CalculateMaterialAmounts(neededVoxels);
                List<int> materialIds = CalculateMaterialIds(neededVoxels);

                for (int i = 0; i < 8; i++)
                {
                    voxelMesh.MaterialAmount.Add(materialAmounts[i]);
                    voxelMesh.MaterialId.Add(materialIds[i]);
                }

                uint id = 0;
                void Add(int index, Vector3 n)
                {
                    voxelMesh.DefaultMesh.Position.Add(internalPositions[index]);
                    voxelMesh.DefaultMesh.Normal.Add(n);
                    voxelMesh.DefaultMesh.IDs.Add(id);
                    voxelMesh.TexCoord3D.Add(internalPositions[index]);
                    voxelMesh.VoxelId.Add(0);
                    id++;
                }

                for (int i = 0; i < vertexNumbers.Count; i += 3)
                {
                    Vector3 n = CalculateNormal(internalPositions[vertexNumbers[i]], internalPositions[vertexNumbers[i + 1]], internalPositions[vertexNumbers[i + 2]]);
                    Add(vertexNumbers[i + 2], n);
                    Add(vertexNumbers[i + 1], n);
                    Add(vertexNumbers[i], n);

                }

                this.Add(voxelMesh.Transform(Matrix4x4.CreateTranslation((Vector3)pos)));
            }
        }

        private List<Voxel> CalculateNeededVoxels(Chunk chunk, Vector3I pos)
        {
            return new List<Voxel>
            {
                chunk[pos],
                chunk[pos + new Vector3I(1, 0, 0)],
                chunk[pos + new Vector3I(1, 1, 0)],
                chunk[pos + new Vector3I(0, 1, 0)],
                chunk[pos + new Vector3I(0, 0, 1)],
                chunk[pos + new Vector3I(1, 0, 1)],
                chunk[pos + new Vector3I(1, 1, 1)],
                chunk[pos + new Vector3I(0, 1, 1)]
            };
        }

        private List<Vector3> CalculateInternalPositions(List<float> internalDistances)
        {
            List<Vector3> internalPositions = new List<Vector3>()
            {
                new Vector3(internalDistances[0], 0.0f, 0.0f),
                new Vector3(1.0f, internalDistances[1], 0.0f),
                new Vector3(internalDistances[2], 1.0f, 0.0f),
                new Vector3(0.0f, internalDistances[3], 0.0f),
                new Vector3(internalDistances[4], 0.0f, 1.0f),
                new Vector3(1.0f, internalDistances[5], 1.0f),
                new Vector3(internalDistances[6], 1.0f, 1.0f),
                new Vector3(0.0f, internalDistances[7], 1.0f),
                new Vector3(0.0f, 0.0f, internalDistances[8]),
                new Vector3(1.0f, 0.0f, internalDistances[9]),
                new Vector3(0.0f, 1.0f, internalDistances[10]),
                new Vector3(1.0f, 1.0f, internalDistances[11])
            };

            return internalPositions;
        }

        private List<float> CalculateInternalDistances(List<Voxel> voxels)
        {
            List<float> internalDistances = new List<float>()
            {
                CalculateInternalDistance(voxels[0],voxels[1]),
                CalculateInternalDistance(voxels[1],voxels[2]),
                CalculateInternalDistance(voxels[3],voxels[2]),
                CalculateInternalDistance(voxels[0],voxels[3]),
                CalculateInternalDistance(voxels[4],voxels[5]),
                CalculateInternalDistance(voxels[5],voxels[6]),
                CalculateInternalDistance(voxels[7],voxels[6]),
                CalculateInternalDistance(voxels[4],voxels[7]),
                CalculateInternalDistance(voxels[0],voxels[4]),
                CalculateInternalDistance(voxels[1],voxels[5]),
                CalculateInternalDistance(voxels[3],voxels[7]),
                CalculateInternalDistance(voxels[2],voxels[6])
            };

            return internalDistances;
        }

        private float CalculateInternalDistance(Voxel first, Voxel second)
        {
            if (first.Exists)
            {
                if (second.Exists)
                {
                    return 0.5f;
                }
                else
                {
                    return CalculateDistanceBetween(first, second);
                }
            }
            else
            {
                if (second.Exists)
                {
                    return 1 - CalculateDistanceBetween(second, first);
                }
                else
                {
                    return 0.5f;
                }
            }
        }

        private float CalculateDistanceBetween(Voxel existing, Voxel other)
        {
            //return (float)(Math.Pow(existing.FillingQuantity, (float)1 / existing.EmptyNeighborCount)*(1+other.FillingQuantity*Math.Pow(2,other.EmptyNeighborCount)) +
            //Math.Pow(other.FillingQuantity, (float)1 / other.EmptyNeighborCount) - 0.5f);

            float existingPart = (existing.FillingQuantity - existing.Threshold) / (1 - existing.Threshold) * sqrtHalf;

            float otherPart = other.FillingQuantity / other.Threshold * (1 - existingPart);

            return existingPart + otherPart;
        }

        private List<float[]> CalculateMaterialAmounts(int[] materialIds, List<Voxel> voxels)
        {
            List<float[]> materialAmounts = new List<float[]>()
            {
                CalculateMaterialAmountsBetween(materialIds, voxels[0], voxels[1]),
                CalculateMaterialAmountsBetween(materialIds, voxels[1], voxels[2]),
                CalculateMaterialAmountsBetween(materialIds, voxels[3], voxels[2]),
                CalculateMaterialAmountsBetween(materialIds, voxels[0], voxels[3]),
                CalculateMaterialAmountsBetween(materialIds, voxels[4], voxels[5]),
                CalculateMaterialAmountsBetween(materialIds, voxels[5], voxels[6]),
                CalculateMaterialAmountsBetween(materialIds, voxels[7], voxels[6]),
                CalculateMaterialAmountsBetween(materialIds, voxels[4], voxels[7]),
                CalculateMaterialAmountsBetween(materialIds, voxels[0], voxels[4]),
                CalculateMaterialAmountsBetween(materialIds, voxels[1], voxels[5]),
                CalculateMaterialAmountsBetween(materialIds, voxels[3], voxels[7]),
                CalculateMaterialAmountsBetween(materialIds, voxels[2], voxels[6])
            };

            return materialAmounts;
        }

        private float[] CalculateMaterialAmountsBetween(int[] materialIds, Voxel voxel1, Voxel voxel2)
        {
            float[] materialAmounts = new float[materialIds.Length];
            float addedFillingQuantity = voxel1.FillingQuantity + voxel2.FillingQuantity;
            for (int i = 0; i < materialIds.Length; i++)
            {
                materialAmounts[i] = 0;
                if (materialIds[i] == voxel1.MaterialId)
                {
                    materialAmounts[i] += voxel1.FillingQuantity / addedFillingQuantity;
                }
                if (materialIds[i] == voxel2.MaterialId)
                {
                    materialAmounts[i] += voxel2.FillingQuantity / addedFillingQuantity;
                }
            }
            return materialAmounts;
        }

        private List<float> CalculateMaterialAmounts(List<Voxel> voxels)
        {
            List<float> materialAmounts = new List<float>();
            foreach (var voxel in voxels)
            {
                materialAmounts.Add(voxel.FillingQuantity);
            }

            return materialAmounts;
        }

        private List<int> CalculateMaterialIds(List<Voxel> voxels)
        {
            List<int> materialIds = new List<int>();
            foreach (var voxel in voxels)
            {
                if (voxel.MaterialId == 0)
                {
                    materialIds.Add(0);
                }
                else
                {
                    materialIds.Add(voxel.MaterialId - 1);
                }
            }

            return materialIds;
        }

        private List<int> GetVertexNumbersFromLookup(List<Voxel> voxels)
        {
            List<int> vertexNumbers = new List<int>();
            int index = CalculateTableIndex(voxels) * 15;
            for (int offset = 0; offset < 15; offset++)
            {
                if (FacesLookup[index + offset] >= 0)
                {
                    vertexNumbers.Add(FacesLookup[index + offset]);
                }
            }
            return vertexNumbers;
        }

        private int CalculateTableIndex(List<Voxel> voxels)
        {
            int index = 0;
            for (int i = 7; i >= 0; i--)
            {
                index <<= 1;
                if (voxels[i].Exists)
                {
                    index++;
                }
            }
            return index;
        }

        private Vector3 CalculateNormal(Vector3 vector1, Vector3 vector2, Vector3 vector3)
        {
            Vector3 v1 = vector2 - vector1;
            Vector3 v2 = vector3 - vector1;
            return Vector3.Normalize(Vector3.Cross(v1, v2));
        }
    }
}
