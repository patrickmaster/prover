using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Prover.UI.Observable
{
	public class ComputedValue<T> : INotifyPropertyChanged, IDisposable
	{
		readonly Dictionary<PropertyChangedEventHandler, WeakReference> _handlers 
					= new Dictionary<PropertyChangedEventHandler, WeakReference>();
 
		readonly Func<T> _valueFunc;

		public T Value
		{
			get
			{
				T result = _valueFunc();
				return result;
			}
		}

		public ComputedValue(Expression<Func<T>> expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}

			Expression body = expression.Body;

			ProcessDependents(body);

			_valueFunc = expression.Compile();
		}

		void ProcessDependents(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					ProcessUnaryExpression((UnaryExpression)expression);
					break;
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.Coalesce:
				case ExpressionType.ArrayIndex:
				case ExpressionType.RightShift:
				case ExpressionType.LeftShift:
				case ExpressionType.ExclusiveOr:
					ProcessBinaryExpression((BinaryExpression)expression);
					break;
//				case ExpressionType.TypeIs:
//					return this.ProcessTypeIs((TypeBinaryExpression)expression);
                case ExpressionType.Conditional:
                    ProcessConditional((ConditionalExpression)expression);
			        break;
//				case ExpressionType.Constant:
//					return this.ProcessConstant((ConstantExpression)expression);
//				case ExpressionType.Parameter:
//					return this.ProcessParameter((ParameterExpression)expression);
				case ExpressionType.MemberAccess:
					ProcessMemberAccessExpression((MemberExpression)expression);
					break;
				case ExpressionType.Call:
					ProcessMethodCallExpression((MethodCallExpression)expression);
					break;
//				case ExpressionType.Lambda:
//					return this.ProcessLambda((LambdaExpression)expression);
//				case ExpressionType.New:
//					return this.ProcessNew((NewExpression)expression);
//				case ExpressionType.NewArrayInit:
//				case ExpressionType.NewArrayBounds:
//					return this.ProcessNewArray((NewArrayExpression)expression);
//				case ExpressionType.Invoke:
//					return this.ProcessInvocation((InvocationExpression)expression);
//				case ExpressionType.MemberInit:
//					return this.ProcessMemberInit((MemberInitExpression)expression);
//				case ExpressionType.ListInit:
//					return this.ProcessListInit((ListInitExpression)expression);
				default:
					return;
			}
		}

	    private void ProcessConditional(ConditionalExpression expression)
	    {
	        if ((bool)GetValue(expression.Test))
	        {
	            ProcessDependents(expression.IfTrue);
	        }
	        else
	        {
	            ProcessDependents(expression.IfFalse);
	        }
	    }

	    void ProcessMethodCallExpression(MethodCallExpression expression)
		{
			foreach (Expression argumentExpression in expression.Arguments)
			{
				ProcessDependents(argumentExpression);
			}
		}

		void ProcessUnaryExpression(UnaryExpression expression)
		{
			ProcessDependents(expression.Operand);
		}

		void ProcessBinaryExpression(BinaryExpression binaryExpression)
		{
			Expression left = binaryExpression.Left;
			Expression right = binaryExpression.Right;
			ProcessDependents(left);
			ProcessDependents(right);
		}

		void ProcessMemberAccessExpression(MemberExpression expression)
		{
			Expression ownerExpression = expression.Expression;
			Type ownerExpressionType = ownerExpression.Type;
											
			if (typeof(INotifyPropertyChanged).IsAssignableFrom(ownerExpressionType))
			{
				try
				{
					string memberName = expression.Member.Name;
					PropertyChangedEventHandler handler = delegate(object sender, PropertyChangedEventArgs args)
						{
							if (args.PropertyName == memberName)
							{
								OnValueChanged();
							}
						};

					var owner = (INotifyPropertyChanged)GetValue(ownerExpression);
					owner.PropertyChanged += handler;

					_handlers[handler] = new WeakReference(owner);
				}
				catch (Exception ex)
				{
					Console.WriteLine("ComputedValue failed to resolve INotifyPropertyChanged value for property {0} {1}", 
					                  expression.Member, ex);
				}
			}
		}

		object GetValue(Expression expression)
		{
			UnaryExpression unaryExpression = Expression.Convert(expression, typeof(object));
			Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(unaryExpression);
			Func<object> getter = getterLambda.Compile();

			return getter();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		void OnValueChanged()
		{
			OnPropertyChanged("Value");
		}

		bool _disposed;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					foreach (KeyValuePair<PropertyChangedEventHandler, WeakReference> pair in _handlers)
					{
						INotifyPropertyChanged target = pair.Value.Target as INotifyPropertyChanged;
						if (target != null)
						{
							target.PropertyChanged -= pair.Key;
						}
					}

					_handlers.Clear();
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}