﻿using HECSFramework.Core;
				HECSDebug.LogWarning($"{Message} {"Float"} {FloatIn.Value(entity)}");

            if (IntIn != null)
                HECSDebug.LogWarning($"{Message} {"Int"} {IntIn.Value(entity)}");

            Next.Execute(entity);