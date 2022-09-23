using Godot;
using System;
using System.Collections.Generic;

namespace Wolf.Scripts
{
	public partial class CharacterActor
	{
        [Flags]
        public enum StateFlags
        {
            None = 0,
            Attacking = 1,
            Ambush = 2
        }

        protected class State
        {
            public string Name;
            public bool Rotated;
            public int[] SpriteIndices;
            public Texture[] SpriteTextures;
            public float Duration;
            public Action<CharacterActor, float> Think;
            public Action<CharacterActor, float> Action;
            public string Next;
            public State NextInstance;
		}

        protected enum GuardSpriteIndex : int
        {
            SPR_GRD_S_1 = 50,
            SPR_GRD_S_2, SPR_GRD_S_3, SPR_GRD_S_4,
            SPR_GRD_S_5, SPR_GRD_S_6, SPR_GRD_S_7, SPR_GRD_S_8,

            SPR_GRD_W1_1, SPR_GRD_W1_2, SPR_GRD_W1_3, SPR_GRD_W1_4,
            SPR_GRD_W1_5, SPR_GRD_W1_6, SPR_GRD_W1_7, SPR_GRD_W1_8,

            SPR_GRD_W2_1, SPR_GRD_W2_2, SPR_GRD_W2_3, SPR_GRD_W2_4,
            SPR_GRD_W2_5, SPR_GRD_W2_6, SPR_GRD_W2_7, SPR_GRD_W2_8,

            SPR_GRD_W3_1, SPR_GRD_W3_2, SPR_GRD_W3_3, SPR_GRD_W3_4,
            SPR_GRD_W3_5, SPR_GRD_W3_6, SPR_GRD_W3_7, SPR_GRD_W3_8,

            SPR_GRD_W4_1, SPR_GRD_W4_2, SPR_GRD_W4_3, SPR_GRD_W4_4,
            SPR_GRD_W4_5, SPR_GRD_W4_6, SPR_GRD_W4_7, SPR_GRD_W4_8,

            SPR_GRD_PAIN_1, SPR_GRD_DIE_1, SPR_GRD_DIE_2, SPR_GRD_DIE_3,
            SPR_GRD_PAIN_2, SPR_GRD_DEAD,

            SPR_GRD_SHOOT1, SPR_GRD_SHOOT2, SPR_GRD_SHOOT3,
        }

        /* Guard, Standing */

        // statetype s_grdstand	= {true,SPR_GRD_S_1,0,T_Stand,NULL,&s_grdstand};
        protected static State STATE_GUARD_STAND = new State()
        {
            Name = nameof(STATE_GUARD_STAND),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_S_1,
                (int)GuardSpriteIndex.SPR_GRD_S_2,
                (int)GuardSpriteIndex.SPR_GRD_S_3,
                (int)GuardSpriteIndex.SPR_GRD_S_4,
                (int)GuardSpriteIndex.SPR_GRD_S_5,
                (int)GuardSpriteIndex.SPR_GRD_S_6,
                (int)GuardSpriteIndex.SPR_GRD_S_7,
                (int)GuardSpriteIndex.SPR_GRD_S_8
            },
            Duration = 0f,
            Think = (a, d) => a.Think_Stand(d),
            Action = null,
            Next = nameof(STATE_GUARD_STAND)
        };

        /* Guard, Path */

        // statetype s_grdpath1 	= {true,SPR_GRD_W1_1,20,T_Path,NULL,&s_grdpath1s};
        protected static State STATE_GUARD_PATH_1 = new State()
        {
            Name = nameof(STATE_GUARD_PATH_1),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W1_1,
                (int)GuardSpriteIndex.SPR_GRD_W1_2,
                (int)GuardSpriteIndex.SPR_GRD_W1_3,
                (int)GuardSpriteIndex.SPR_GRD_W1_4,
                (int)GuardSpriteIndex.SPR_GRD_W1_5,
                (int)GuardSpriteIndex.SPR_GRD_W1_6,
                (int)GuardSpriteIndex.SPR_GRD_W1_7,
                (int)GuardSpriteIndex.SPR_GRD_W1_8
            },
            Duration = 0.2f,
            Think = (a, d) => a.Think_Path(d),
            Action = null,
            Next = nameof(STATE_GUARD_PATH_1S)
        };

        // statetype s_grdpath1s 	= {true,SPR_GRD_W1_1,5,NULL,NULL,&s_grdpath2};
        protected static State STATE_GUARD_PATH_1S = new State()
        {
            Name = nameof(STATE_GUARD_PATH_1S),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W1_1,
                (int)GuardSpriteIndex.SPR_GRD_W1_2,
                (int)GuardSpriteIndex.SPR_GRD_W1_3,
                (int)GuardSpriteIndex.SPR_GRD_W1_4,
                (int)GuardSpriteIndex.SPR_GRD_W1_5,
                (int)GuardSpriteIndex.SPR_GRD_W1_6,
                (int)GuardSpriteIndex.SPR_GRD_W1_7,
                (int)GuardSpriteIndex.SPR_GRD_W1_8
            },
            Duration = 0.05f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_PATH_2)
        };

        // statetype s_grdpath2 	= {true,SPR_GRD_W2_1,15,T_Path,NULL,&s_grdpath3};
        protected static State STATE_GUARD_PATH_2 = new State()
        {
            Name = nameof(STATE_GUARD_PATH_2),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W2_1,
                (int)GuardSpriteIndex.SPR_GRD_W2_2,
                (int)GuardSpriteIndex.SPR_GRD_W2_3,
                (int)GuardSpriteIndex.SPR_GRD_W2_4,
                (int)GuardSpriteIndex.SPR_GRD_W2_5,
                (int)GuardSpriteIndex.SPR_GRD_W2_6,
                (int)GuardSpriteIndex.SPR_GRD_W2_7,
                (int)GuardSpriteIndex.SPR_GRD_W2_8
            },
            Duration = 0.15f,
            Think = (a, d) => a.Think_Path(d),
            Action = null,
            Next = nameof(STATE_GUARD_PATH_3)
        };

        // statetype s_grdpath3 	= {true,SPR_GRD_W3_1,20,T_Path,NULL,&s_grdpath3s};
        protected static State STATE_GUARD_PATH_3 = new State()
        {
            Name = nameof(STATE_GUARD_PATH_3),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W3_1,
                (int)GuardSpriteIndex.SPR_GRD_W3_2,
                (int)GuardSpriteIndex.SPR_GRD_W3_3,
                (int)GuardSpriteIndex.SPR_GRD_W3_4,
                (int)GuardSpriteIndex.SPR_GRD_W3_5,
                (int)GuardSpriteIndex.SPR_GRD_W3_6,
                (int)GuardSpriteIndex.SPR_GRD_W3_7,
                (int)GuardSpriteIndex.SPR_GRD_W3_8
            },
            Duration = 0.2f,
            Think = (a, d) => a.Think_Path(d),
            Action = null,
            Next = nameof(STATE_GUARD_PATH_3S)
        };

        // statetype s_grdpath3s 	= {true,SPR_GRD_W3_1,5,NULL,NULL,&s_grdpath4};
        protected static State STATE_GUARD_PATH_3S = new State()
        {
            Name = nameof(STATE_GUARD_PATH_3S),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W3_1,
                (int)GuardSpriteIndex.SPR_GRD_W3_2,
                (int)GuardSpriteIndex.SPR_GRD_W3_3,
                (int)GuardSpriteIndex.SPR_GRD_W3_4,
                (int)GuardSpriteIndex.SPR_GRD_W3_5,
                (int)GuardSpriteIndex.SPR_GRD_W3_6,
                (int)GuardSpriteIndex.SPR_GRD_W3_7,
                (int)GuardSpriteIndex.SPR_GRD_W3_8
            },
            Duration = 0.05f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_PATH_4)
        };

        // statetype s_grdpath4 	= {true,SPR_GRD_W4_1,15,T_Path,NULL,&s_grdpath1};
        protected static State STATE_GUARD_PATH_4 = new State()
        {
            Name = nameof(STATE_GUARD_PATH_4),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W4_1,
                (int)GuardSpriteIndex.SPR_GRD_W4_2,
                (int)GuardSpriteIndex.SPR_GRD_W4_3,
                (int)GuardSpriteIndex.SPR_GRD_W4_4,
                (int)GuardSpriteIndex.SPR_GRD_W4_5,
                (int)GuardSpriteIndex.SPR_GRD_W4_6,
                (int)GuardSpriteIndex.SPR_GRD_W4_7,
                (int)GuardSpriteIndex.SPR_GRD_W4_8
            },
            Duration = 0.15f,
            Think = (a, d) => a.Think_Path(d),
            Action = null,
            Next = nameof(STATE_GUARD_PATH_1)
        };

        /* Guard, Pain */

        //statetype s_grdpain = { 2, SPR_GRD_PAIN_1, 10, NULL, NULL, &s_grdchase1 };
        protected static State STATE_GUARD_PAIN = new State()
        {
            Name = nameof(STATE_GUARD_PAIN),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_PAIN_1
            },
            Duration = 0.1f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_1)
        };

        //statetype s_grdpain1 = { 2, SPR_GRD_PAIN_2, 10, NULL, NULL, &s_grdchase1 };
        protected static State STATE_GUARD_PAIN_1 = new State()
        {
            Name = nameof(STATE_GUARD_PAIN_1),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_PAIN_2
            },
            Duration = 0.10f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_1)
        };

        /* Guard, Shoot */

        //statetype s_grdshoot1 = { false, SPR_GRD_SHOOT1, 20, NULL, NULL, &s_grdshoot2 };
        protected static State STATE_GUARD_SHOOT_1 = new State()
        {
            Name = nameof(STATE_GUARD_SHOOT_1),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_SHOOT1
            },
            Duration = 0.28f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_SHOOT_2)
        };

        //statetype s_grdshoot2 = { false, SPR_GRD_SHOOT2, 20, NULL, T_Shoot, &s_grdshoot3 };
        protected static State STATE_GUARD_SHOOT_2 = new State()
        {
            Name = nameof(STATE_GUARD_SHOOT_2),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_SHOOT2
            },
            Duration = 0.28f,
            Think = null,
            Action = (a, d) => a.Act_Shoot(d),
            Next = nameof(STATE_GUARD_SHOOT_3)
        };

        //statetype s_grdshoot3 = { false, SPR_GRD_SHOOT3, 20, NULL, NULL, &s_grdchase1 };
        protected static State STATE_GUARD_SHOOT_3 = new State()
        {
            Name = nameof(STATE_GUARD_SHOOT_3),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_SHOOT3
            },
            Duration = 0.28f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_1)
        };

        /* Guard, Chase */

        //statetype s_grdchase1 = { true, SPR_GRD_W1_1, 10, T_Chase, NULL, &s_grdchase1s };
        protected static State STATE_GUARD_CHASE_1 = new State()
        {
            Name = nameof(STATE_GUARD_CHASE_1),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W1_1,
                (int)GuardSpriteIndex.SPR_GRD_W1_2,
                (int)GuardSpriteIndex.SPR_GRD_W1_3,
                (int)GuardSpriteIndex.SPR_GRD_W1_4,
                (int)GuardSpriteIndex.SPR_GRD_W1_5,
                (int)GuardSpriteIndex.SPR_GRD_W1_6,
                (int)GuardSpriteIndex.SPR_GRD_W1_7,
                (int)GuardSpriteIndex.SPR_GRD_W1_8
            },
            Duration = 0.14f,
            Think = (a, d) => a.Think_Chase(d),
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_1S)
        };

        //statetype s_grdchase1s = { true, SPR_GRD_W1_1, 3, NULL, NULL, &s_grdchase2 };
        protected static State STATE_GUARD_CHASE_1S = new State()
        {
            Name = nameof(STATE_GUARD_CHASE_1S),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W1_1,
                (int)GuardSpriteIndex.SPR_GRD_W1_2,
                (int)GuardSpriteIndex.SPR_GRD_W1_3,
                (int)GuardSpriteIndex.SPR_GRD_W1_4,
                (int)GuardSpriteIndex.SPR_GRD_W1_5,
                (int)GuardSpriteIndex.SPR_GRD_W1_6,
                (int)GuardSpriteIndex.SPR_GRD_W1_7,
                (int)GuardSpriteIndex.SPR_GRD_W1_8
            },
            Duration = 0.04f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_2)
        };

        //statetype s_grdchase2 = { true, SPR_GRD_W2_1, 8, T_Chase, NULL, &s_grdchase3 };
        protected static State STATE_GUARD_CHASE_2 = new State()
        {
            Name = nameof(STATE_GUARD_CHASE_2),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W2_1,
                (int)GuardSpriteIndex.SPR_GRD_W2_2,
                (int)GuardSpriteIndex.SPR_GRD_W2_3,
                (int)GuardSpriteIndex.SPR_GRD_W2_4,
                (int)GuardSpriteIndex.SPR_GRD_W2_5,
                (int)GuardSpriteIndex.SPR_GRD_W2_6,
                (int)GuardSpriteIndex.SPR_GRD_W2_7,
                (int)GuardSpriteIndex.SPR_GRD_W2_8
            },
            Duration = 0.11f,
            Think = (a, d) => a.Think_Chase(d),
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_3)
        };

        //statetype s_grdchase3 = { true, SPR_GRD_W3_1, 10, T_Chase, NULL, &s_grdchase3s };
        protected static State STATE_GUARD_CHASE_3 = new State()
        {
            Name = nameof(STATE_GUARD_CHASE_3),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W3_1,
                (int)GuardSpriteIndex.SPR_GRD_W3_2,
                (int)GuardSpriteIndex.SPR_GRD_W3_3,
                (int)GuardSpriteIndex.SPR_GRD_W3_4,
                (int)GuardSpriteIndex.SPR_GRD_W3_5,
                (int)GuardSpriteIndex.SPR_GRD_W3_6,
                (int)GuardSpriteIndex.SPR_GRD_W3_7,
                (int)GuardSpriteIndex.SPR_GRD_W3_8
            },
            Duration = 0.14f,
            Think = (a, d) => a.Think_Chase(d),
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_3S)
        };

        //statetype s_grdchase3s = { true, SPR_GRD_W3_1, 3, NULL, NULL, &s_grdchase4 };
        protected static State STATE_GUARD_CHASE_3S = new State()
        {
            Name = nameof(STATE_GUARD_CHASE_3S),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W3_1,
                (int)GuardSpriteIndex.SPR_GRD_W3_2,
                (int)GuardSpriteIndex.SPR_GRD_W3_3,
                (int)GuardSpriteIndex.SPR_GRD_W3_4,
                (int)GuardSpriteIndex.SPR_GRD_W3_5,
                (int)GuardSpriteIndex.SPR_GRD_W3_6,
                (int)GuardSpriteIndex.SPR_GRD_W3_7,
                (int)GuardSpriteIndex.SPR_GRD_W3_8
            },
            Duration = 0.04f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_4)
        };

        //statetype s_grdchase4 = { true, SPR_GRD_W4_1, 8, T_Chase, NULL, &s_grdchase1 };
        protected static State STATE_GUARD_CHASE_4 = new State()
        {
            Name = nameof(STATE_GUARD_CHASE_4),
            Rotated = true,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_W4_1,
                (int)GuardSpriteIndex.SPR_GRD_W4_2,
                (int)GuardSpriteIndex.SPR_GRD_W4_3,
                (int)GuardSpriteIndex.SPR_GRD_W4_4,
                (int)GuardSpriteIndex.SPR_GRD_W4_5,
                (int)GuardSpriteIndex.SPR_GRD_W4_6,
                (int)GuardSpriteIndex.SPR_GRD_W4_7,
                (int)GuardSpriteIndex.SPR_GRD_W4_8
            },
            Duration = 0.11f,
            Think = (a, d) => a.Think_Chase(d),
            Action = null,
            Next = nameof(STATE_GUARD_CHASE_1)
        };

        /* Guard, Die */

        //statetype s_grddie1 = { false, SPR_GRD_DIE_1, 15, NULL, A_DeathScream, &s_grddie2 };
        protected static State STATE_GUARD_DIE = new State()
        {
            Name = nameof(STATE_GUARD_DIE),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_DIE_1
            },
            Duration = 0.15f,
            Think = null,
            Action = (a, d) => a.Act_Scream(d),
            Next = nameof(STATE_GUARD_DIE_2)
        };

        //statetype s_grddie2 = { false, SPR_GRD_DIE_2, 15, NULL, NULL, &s_grddie3 };
        protected static State STATE_GUARD_DIE_2 = new State()
        {
            Name = nameof(STATE_GUARD_DIE_2),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_DIE_2
            },
            Duration = 0.15f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_DIE_3)
        };

        //statetype s_grddie3 = { false, SPR_GRD_DIE_3, 15, NULL, NULL, &s_grddie4 };
        protected static State STATE_GUARD_DIE_3 = new State()
        {
            Name = nameof(STATE_GUARD_DIE_3),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_DIE_3
            },
            Duration = 0.15f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_DIE_4)
        };

        //statetype s_grddie4 = { false, SPR_GRD_DEAD, 0, NULL, NULL, &s_grddie4 };
        protected static State STATE_GUARD_DIE_4 = new State()
        {
            Name = nameof(STATE_GUARD_DIE_4),
            Rotated = false,
            SpriteIndices = new int[] {
                (int)GuardSpriteIndex.SPR_GRD_DEAD
            },
            Duration = 0f,
            Think = null,
            Action = null,
            Next = nameof(STATE_GUARD_DIE_4)
        };
    }
}
