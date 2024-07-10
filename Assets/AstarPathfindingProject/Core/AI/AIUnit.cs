using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    using Pathfinding.RVO;
    using Pathfinding.Util;
    using System;

    public class AIUnit :AIPath
    {
        public event EventHandler OnDestinationReached;

        public override void OnTargetReached()
        {
            OnDestinationReached?.Invoke(this, EventArgs.Empty);
        }
    }
}
