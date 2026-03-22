using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.Globalization;

namespace ExpressionEvaluator.Core;

public class Evaluator
{
    public static double Evaluate(string infix)
    {
        var tokens = Tokenize(infix); 
        var postfix = InfixToPostfix(infix);
        return EvaluatePostfix(postfix);
    }

    private static List<string> Tokenize(string infix)
    {
        var tokens = new List<string>();

        for (int i = 0; i < infix.Length; i++)
        {
            if (char.IsDigit(infix[i]) || infix[i] == '.')
            {
                var number = string.Empty;
                while (i < infix.Length && (char.IsDigit(infix[i]) || infix[i] == '.'))
                {
                    number += infix[i];
                    i++;
                }
                tokens.Add(number);
                i--; 
            }
            else if ("+-*/^()".Contains(infix[i]))
            {
                tokens.Add(infix[i].ToString());
            }
        }
        return tokens;
    }
    private static string InfixToPostfix(string infix)
    {
        var postFix = string.Empty;
        var stack = new Stack<string>();
        var tokens = Tokenize(infix);

        foreach (var token in tokens)
        {
            if (IsOperator(token))
            {
                if (stack.Count == 0)
                {
                    stack.Push(token);
                }
                else
                {
                    if (token == ")")
                    {
                        do
                        {
                            postFix += stack.Pop() + " ";
                        } while (stack.Peek() != "(");
                        stack.Pop();
                    }
                    else
                    {
                        if (PriorityInfix(token) > PriorityStack(stack.Peek()))
                        {
                            stack.Push(token);
                        }
                        else
                        {
                            postFix += stack.Pop() + " ";
                            stack.Push(token);
                        }
                    }
                }
            }
            else
            {
                postFix += token + " ";
            }
        }
        while (stack.Count > 0)
        {
            postFix += stack.Pop() + " ";
        }
        return postFix;
    }

    private static int PriorityStack(string item) => item switch
    {
        "^" => 3,
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        "(" => 0,
        _ => throw new Exception("Sintax error."),
    };

    private static int PriorityInfix(string item) => item switch
    {
        "^" => 4,
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        "(" => 5,
        _ => throw new Exception("Sintax error."),
    };

    private static double EvaluatePostfix(string postfix)
    {
        var stack = new Stack<double>();
        var tokens = postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            if (IsOperator(token))
            {
                var b = stack.Pop();
                var a = stack.Pop();
                stack.Push(token switch
                {
                    "+" => a + b,
                    "-" => a - b,
                    "*" => a * b,
                    "/" => a / b,
                    "^" => Math.Pow(a, b),
                    _ => throw new Exception("Sintax error."),
                });
            }
            else
            {
                stack.Push(double.Parse(token, CultureInfo.InvariantCulture));
            }
        }
        return stack.Pop();
    }

    private static bool IsOperator(string item) => "+-*/^()".Contains(item);
}