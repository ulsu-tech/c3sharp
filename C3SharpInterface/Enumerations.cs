/*
 * Copyright (c) 2020-2022 Dmitry Lavygin <vdm.inbox@gmail.com>
 * S.P. Kapitsa Research Institute of Technology of Ulyanovsk State University.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     1. Redistributions of source code must retain the above copyright
 *        notice, this list of conditions and the following disclaimer.
 *
 *     2. Redistributions in binary form must reproduce the above copyright
 *        notice, this list of conditions and the following disclaimer in the
 *        documentation and/or other materials provided with the distribution.
 *
 *     3. Neither the name of the copyright holder nor the names of its
 *        contributors may be used to endorse or promote products derived from
 *        this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace C3SharpInterface
{
    public enum Error
    {
        General = 0,
        Success = 1,
        Access = 2,
        Argument = 3,
        Memory = 4,
        Pointer = 5,
        Unexpected = 6,
        NotImplemented = 7,
        NoInterface = 8,
        Protocol = 9,
        LongAnswer = 10
    };

    public enum Command
    {
        Invalid = -1,

        // Variables Handling
        ReadVariableAscii = 0,
        WriteVariableAscii = 1,
        ReadArrayAscii = 2,
        WriteArrayAscii = 3,
        ReadVariable = 4,
        WriteVariable = 5,
        ReadMultiple = 6,
        WriteMultiple = 7,

        // Reserved
        Reserved8 = 8,
        Reserved9 = 9,

        // Program Handling
        ProgramControl = 10,

        // Motion Handling
        Motion = 11,

        // KCP Key Handling
        KcpAction = 12,

        // Proxy Information Handling
        ProxyInfo = 13,
        ProxyFeatures = 14,
        ProxyInfoEx = 15,
        ProxyCrossInfo = 16,
        ProxyBenchmark = 17,

        // Reserved
        Reserved18 = 18,
        Reserved19 = 19,

        // File Handling
        FileSetAttribute = 20,
        FileNameList = 21,
        FileCreate = 22,
        FileDelete = 23,
        FileCopy = 24,
        FileMove = 25,
        FileGetProperties = 26,
        FileGetFullName = 27,
        FileGetKrcName = 28,
        FileWriteContent = 29,
        FileReadContent = 30,

        // Reserved
        Reserved31 = 31,
        Reserved32 = 32,
        Reserved33 = 33,
        Reserved34 = 34,
        Reserved35 = 35,
        Reserved36 = 36,
        Reserved37 = 37,
        Reserved38 = 38,
        Reserved39 = 39,
        Reserved40 = 40,
        Reserved41 = 41,
        Reserved42 = 42,
        Reserved43 = 43,
        Reserved44 = 44,
        Reserved45 = 45,
        Reserved46 = 46,
        Reserved47 = 47,
        Reserved48 = 48,
        Reserved49 = 49,

        // CrossCommEXE Functions Handling
        CrossSetInfoOn = 50,
        CrossSetInfoOff = 51,
        CrossGetRobotDirectory = 52,
        CrossDownloadDiskToRobot = 53,
        CrossDownloadMemToRobot = 54,
        CrossUploadFromRobotToDisk = 55,
        CrossUploadFromRobotToMem = 56,
        CrossDeleteRobotProgram = 57,
        CrossRobotLevelStop = 58,
        CrossControlLevelStop = 59,
        CrossRunControlLevel = 60,
        CrossSelectModul = 61,
        CrossCancelModul = 62,
        CrossConfirmAll = 63,
        CrossKrcOk = 64,
        CrossIoRestart = 65,
        CrossReserved66 = 66,
        CrossReserved67 = 67,
        CrossReserved68 = 68,
        CrossReserved69 = 69,

        // Reserved
        Reserved70 = 70,
        Reserved71 = 71,
        Reserved72 = 72,
        Reserved73 = 73,
        Reserved74 = 74,
        Reserved75 = 75,
        Reserved76 = 76,
        Reserved77 = 77,
        Reserved78 = 78,
        Reserved79 = 79,
        Reserved80 = 80,
        Reserved81 = 81,
        Reserved82 = 82,
        Reserved83 = 83,
        Reserved84 = 84,
        Reserved85 = 85,
        Reserved86 = 86,
        Reserved87 = 87,
        Reserved88 = 88,
        Reserved89 = 89,
        Reserved90 = 90,
        Reserved91 = 91,
        Reserved92 = 92,
        Reserved93 = 93,
        Reserved94 = 94,
        Reserved95 = 95,
        Reserved96 = 96,
        Reserved97 = 97,
        Reserved98 = 98,
        Reserved99 = 99,
        Reserved100 = 100,
        Reserved101 = 101,
        Reserved102 = 102,
        Reserved103 = 103,
        Reserved104 = 104,
        Reserved105 = 105,
        Reserved106 = 106,
        Reserved107 = 107,
        Reserved108 = 108,
        Reserved109 = 109,
        Reserved110 = 110,
        Reserved111 = 111,
        Reserved112 = 112,
        Reserved113 = 113,
        Reserved114 = 114,
        Reserved115 = 115,
        Reserved116 = 116,
        Reserved117 = 117,
        Reserved118 = 118,
        Reserved119 = 119,
        Reserved120 = 120,
        Reserved121 = 121,
        Reserved122 = 122,
        Reserved123 = 123,
        Reserved124 = 124,
        Reserved125 = 125,
        Reserved126 = 126,
        Reserved127 = 127,
        Reserved128 = 128,

        // Extended Command
        Extended = 255
    };

    public enum ProgramControl
    {
        None = 0,
        Reset = 1,
        Start = 2,
        Stop = 3,
        Cancel = 4,
        Select = 5,
        Run = 6,
        DisableStart = 7,
        ResetSumbit = 8,
        StartSumbit = 9,
        StopSumbit = 10,
        CancelSumbit = 11,
        SelectSumbit = 12,
        RunSumbit = 13
    };

    public enum MotionType
    {
        None = 0,
        Ptp = 1,
        Lin = 2,
        PtpRelative = 3,
        LinRelative = 4
    };

    public enum KcpActionType
    {
        None = 0,
        Start = 1,
        Stop = 2,
        Move = 3,
        Move6D = 4
    };

    public enum KcpKeyDirection
    {
        Forward,
        Backward
    }

    public enum KcpKeyStatus
    {
        Pressed,
        Released
    }

    [Flags]
    public enum ItemAttribute
    {
        None = 0,
        ReadOnly = 1,
        Hidden = 2,
        System = 4,
        Directory = 16,
        Archiv = 32,
        Encrypted = 16384,
        Signed = 0x10000000
    }

    [Flags]
    public enum ItemType
    {
        Unknown = 0,
        Directory = 1,
        VirtualDirectory = 2,
        Archive = 4,
        BinaryFile = 8,
        TextFile = 16,
        Module = 32,
        Raw = 64,
        MotionFile = 128,
        ProtectedFileContainer = 256
    }

    [Flags]
    public enum ItemListFlag
    {
        None = 0,
        Recursive = 1,
        Expand = 2,
        Long = 4,
        OldLong = 8,
        NoPFC = 16,
        PFCAsFile = 32,
        ZIPAsFile = 64
    }

    [Flags]
    public enum ModulePart
    {
        Unknown = 0,
        SUB = 1,
        SRC = 2,
        DAT = 4,
        SUBDAT = 5,
        SRCDAT = 6,
        Template = 8,
        Motion = 16
    }

    [Flags]
    public enum CopyFlag
    {
        None = 0,
        Archive = 1,
        Modify = 3,
        Continue = 4,
        Recursive = 8,
        Refresh = 16,
        Update = 48,
        OverwriteExist = 64,
        NoDirectoryEntries = 128,
        JunkDirectory = 256,
        ForceBinary = 512,
        ForceText = 1024,
        NoVersionCheck = 2048,
        OverwriteReadonly = 4096,
        NoKrlAnalysis = 8192
    }

    [Flags]
    public enum ItemPropertyFlag
    {
        None = 0,
        Type = 1,
        Name = 2,
        Size = 4,
        Attributes = 8,
        TimeCreated = 16,
        TimeAccess = 32,
        TimeModified = 64,
        EditMode = 128,
        All = EditMode | TimeModified | TimeAccess | TimeCreated | Attributes | Size | Name | Type
    }

    public enum EditMode
    {
        Unknown = -1,
        FullEdit = 0,
        DatKor = 1,
        ProKor = 2,
        ReadOnly = 3
    }

    public enum PathType
    {
        FullPath,
        KrcPath
    }

    public enum FileIoOperation
    {
        None = 0,
        Begin = 1,
        Data = 2,
        GetSize = 3,
        End = 4,
        Checksum = 5
    };
}
