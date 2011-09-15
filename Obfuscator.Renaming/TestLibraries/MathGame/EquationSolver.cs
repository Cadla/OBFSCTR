using System;
using System.Collections.Generic;
using System.Text;

namespace MathGame
{
    public static class EquationSolver
    {
        public static double SolvePar(string equation)
        {
            int _indexA = -1;
            int _indexB = -1;

            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i].Equals('('))
                {
                    _indexA = i;
                }
                else if (equation[i].Equals(')'))
                {
                    _indexB = i;
                    break;
                }
            }

            //System.Console.WriteLine("Index of first is {0}, and second is {1}.", _indexA, _indexB);

            if (_indexA != -1 || _indexB != -1)
            {
                string _replace = equation.Substring(_indexA, _indexB - _indexA + 1);
                string _replacement = equation.Substring(_indexA + 1, _indexB - _indexA - 1);

                //Console.WriteLine("Replace: " + _replace);
                //Console.WriteLine("Replacement: " + _replacement);

                return SolvePar(equation.Replace(_replace, SolvePar(_replacement).ToString()));
            }
            else
                return Solve(equation);
        }

        public static double Solve(string equation)
        {
            if (String.IsNullOrEmpty(equation))
                return 0;

            // ^, *, /, +, -
            char[] _seps = new char[1];
            string[] sides = new string[1];

            sides[0] = equation;

            if (equation.Contains("+"))
            {
                _seps[0] = '+';
                sides = equation.Split(_seps, 2);
            }
            else if (equation.Contains("-"))
            {
                _seps[0] = '-';
                sides = equation.Split(_seps, 2);
            }
            else if (equation.Contains("x"))
            {
                _seps[0] = 'x';
                sides = equation.Split(_seps, 2);
            }
            else if (equation.Contains("/"))
            {
                _seps[0] = '/';
                sides = equation.Split(_seps, 2);
            }

            if (sides.Length == 1)
            {
                if (sides[0].Contains("+") || sides[0].Contains("-") || sides[0].Contains("/"))
                    return Solve(sides[0]);

                return double.Parse(sides[0]);
            }

            //Console.WriteLine("Sides [{0}] and [{1}].", sides[0], sides[1]);
            if (_seps[0] == 'x')
                return Solve(sides[0]) * Solve(sides[1]);
            else if (_seps[0] == '/')
            {
                if (Solve(sides[1]) == 0)
                    return 0;

                return Solve(sides[0]) / Solve(sides[1]);
            }
            else if (_seps[0] == '+')
                return Solve(sides[0]) + Solve(sides[1]);
            else if (_seps[0] == '-')
                return Solve(sides[0]) - Solve(sides[1]);

            return 0;
        }
    }
}
