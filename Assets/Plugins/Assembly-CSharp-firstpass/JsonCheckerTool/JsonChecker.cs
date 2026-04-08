using System;
using System.Collections.Generic;

namespace JsonCheckerTool
{
	internal sealed class JsonChecker
	{
		[Serializable]
		private enum Mode
		{
			Array = 0,
			Done = 1,
			Key = 2,
			Object = 3
		}

		private const int __ = -1;

		private const int C_SPACE = 0;

		private const int C_WHITE = 1;

		private const int C_LCURB = 2;

		private const int C_RCURB = 3;

		private const int C_LSQRB = 4;

		private const int C_RSQRB = 5;

		private const int C_COLON = 6;

		private const int C_COMMA = 7;

		private const int C_QUOTE = 8;

		private const int C_BACKS = 9;

		private const int C_SLASH = 10;

		private const int C_PLUS = 11;

		private const int C_MINUS = 12;

		private const int C_POINT = 13;

		private const int C_ZERO = 14;

		private const int C_DIGIT = 15;

		private const int C_LOW_A = 16;

		private const int C_LOW_B = 17;

		private const int C_LOW_C = 18;

		private const int C_LOW_D = 19;

		private const int C_LOW_E = 20;

		private const int C_LOW_F = 21;

		private const int C_LOW_L = 22;

		private const int C_LOW_N = 23;

		private const int C_LOW_R = 24;

		private const int C_LOW_S = 25;

		private const int C_LOW_T = 26;

		private const int C_LOW_U = 27;

		private const int C_ABCDF = 28;

		private const int C_E = 29;

		private const int C_ETC = 30;

		private const int NR_CLASSES = 31;

		private const int GO = 0;

		private const int OK = 1;

		private const int OB = 2;

		private const int KE = 3;

		private const int CO = 4;

		private const int VA = 5;

		private const int AR = 6;

		private const int ST = 7;

		private const int ES = 8;

		private const int U1 = 9;

		private const int U2 = 10;

		private const int U3 = 11;

		private const int U4 = 12;

		private const int MI = 13;

		private const int ZE = 14;

		private const int IN = 15;

		private const int FR = 16;

		private const int E1 = 17;

		private const int E2 = 18;

		private const int E3 = 19;

		private const int T1 = 20;

		private const int T2 = 21;

		private const int T3 = 22;

		private const int F1 = 23;

		private const int F2 = 24;

		private const int F3 = 25;

		private const int F4 = 26;

		private const int N1 = 27;

		private const int N2 = 28;

		private const int N3 = 29;

		private const int NR_STATES = 30;

		public const int NoDepthLimit = 0;

		private int _state;

		private long _offset;

		private readonly int _depth;

		private readonly Stack<Mode> _stack;

		private static readonly int[] ascii_class = new int[128]
		{
			-1, -1, -1, -1, -1, -1, -1, -1, -1, 1,
			1, -1, -1, 1, -1, -1, -1, -1, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
			-1, -1, 0, 30, 8, 30, 30, 30, 30, 30,
			30, 30, 30, 11, 7, 12, 13, 10, 14, 15,
			15, 15, 15, 15, 15, 15, 15, 15, 6, 30,
			30, 30, 30, 30, 30, 28, 28, 28, 28, 29,
			28, 30, 30, 30, 30, 30, 30, 30, 30, 30,
			30, 30, 30, 30, 30, 30, 30, 30, 30, 30,
			30, 4, 9, 5, 30, 30, 30, 16, 17, 18,
			19, 20, 21, 30, 30, 30, 30, 30, 22, 30,
			23, 30, 30, 30, 24, 25, 26, 27, 30, 30,
			30, 30, 30, 2, 30, 3, 30, 30
		};

		private static readonly int[,] state_transition_table = new int[30, 31]
		{
			{
				0, 0, -6, -1, -5, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				1, 1, -1, -8, -1, -7, -1, -3, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				2, 2, -1, -9, -1, -1, -1, -1, 7, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				3, 3, -1, -1, -1, -1, -1, -1, 7, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				4, 4, -1, -1, -1, -1, -2, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				5, 5, -6, -1, -5, -1, -1, -1, 7, -1,
				-1, -1, 13, -1, 14, 15, -1, -1, -1, -1,
				-1, 23, -1, 27, -1, -1, 20, -1, -1, -1,
				-1
			},
			{
				6, 6, -6, -1, -5, -7, -1, -1, 7, -1,
				-1, -1, 13, -1, 14, 15, -1, -1, -1, -1,
				-1, 23, -1, 27, -1, -1, 20, -1, -1, -1,
				-1
			},
			{
				7, -1, 7, 7, 7, 7, 7, 7, -4, 8,
				7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
				7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
				7
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, 7, 7,
				7, -1, -1, -1, -1, -1, -1, 7, -1, -1,
				-1, 7, -1, 7, 7, -1, 7, 9, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 10, 10, 10, 10, 10, 10,
				10, 10, -1, -1, -1, -1, -1, -1, 10, 10,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 11, 11, 11, 11, 11, 11,
				11, 11, -1, -1, -1, -1, -1, -1, 11, 11,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 12, 12, 12, 12, 12, 12,
				12, 12, -1, -1, -1, -1, -1, -1, 12, 12,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 7, 7, 7, 7, 7, 7,
				7, 7, -1, -1, -1, -1, -1, -1, 7, 7,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 14, 15, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				1, 1, -1, -8, -1, -7, -1, -3, -1, -1,
				-1, -1, -1, 16, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				1, 1, -1, -8, -1, -7, -1, -3, -1, -1,
				-1, -1, -1, 16, 15, 15, -1, -1, -1, -1,
				17, -1, -1, -1, -1, -1, -1, -1, -1, 17,
				-1
			},
			{
				1, 1, -1, -8, -1, -7, -1, -3, -1, -1,
				-1, -1, -1, -1, 16, 16, -1, -1, -1, -1,
				17, -1, -1, -1, -1, -1, -1, -1, -1, 17,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, 18, 18, -1, 19, 19, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 19, 19, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				1, 1, -1, -8, -1, -7, -1, -3, -1, -1,
				-1, -1, -1, -1, 19, 19, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 21, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, 22, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, 24, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, 25, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, 26, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, 28, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, 29, -1, -1, -1, -1, -1, -1, -1,
				-1
			},
			{
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, 1, -1, -1, -1, -1, -1, -1, -1,
				-1
			}
		};

		public JsonChecker()
			: this(0)
		{
		}

		public JsonChecker(int depth)
		{
			if (depth < 0)
			{
				throw new ArgumentOutOfRangeException("depth", depth, null);
			}
			_state = 0;
			_depth = depth;
			_stack = new Stack<Mode>(depth);
			Push(Mode.Done);
		}

		public void Check(int ch)
		{
			if (ch < 0)
			{
				OnError();
			}
			int num;
			if (ch >= 128)
			{
				num = 30;
			}
			else
			{
				num = ascii_class[ch];
				if (num <= -1)
				{
					OnError();
				}
			}
			int num2 = state_transition_table[_state, num];
			if (num2 >= 0)
			{
				_state = num2;
			}
			else
			{
				switch (num2)
				{
				case -9:
					Pop(Mode.Key);
					_state = 1;
					break;
				case -8:
					Pop(Mode.Object);
					_state = 1;
					break;
				case -7:
					Pop(Mode.Array);
					_state = 1;
					break;
				case -6:
					Push(Mode.Key);
					_state = 2;
					break;
				case -5:
					Push(Mode.Array);
					_state = 6;
					break;
				case -4:
					switch (_stack.Peek())
					{
					case Mode.Key:
						_state = 4;
						break;
					case Mode.Array:
					case Mode.Object:
						_state = 1;
						break;
					default:
						OnError();
						break;
					}
					break;
				case -3:
					switch (_stack.Peek())
					{
					case Mode.Object:
						Pop(Mode.Object);
						Push(Mode.Key);
						_state = 3;
						break;
					case Mode.Array:
						_state = 5;
						break;
					default:
						OnError();
						break;
					}
					break;
				case -2:
					Pop(Mode.Key);
					Push(Mode.Object);
					_state = 5;
					break;
				default:
					OnError();
					break;
				}
			}
			_offset++;
		}

		public void FinalCheck()
		{
			if (_state != 1)
			{
				OnError();
			}
			Pop(Mode.Done);
		}

		private void Push(Mode mode)
		{
			if (_depth > 0 && _stack.Count >= _depth)
			{
				OnError();
			}
			_stack.Push(mode);
		}

		private void Pop(Mode mode)
		{
			if (_stack.Pop() != mode)
			{
				OnError();
			}
		}

		private void OnError()
		{
			throw new Exception(string.Format("Invalid JSON text at character offset {0}.", _offset.ToString("N0")));
		}
	}
}
