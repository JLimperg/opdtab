using System;
namespace OPDtabData
{
	public class Either<T, U>
	{
		readonly T left;
		readonly U right;
		readonly bool isLeft;

		Either(T left)
		{
			this.left = left;
			isLeft = true;
		}

		Either(U right)
		{
			this.right = right;
			isLeft = false;
		}

		public static Either<T, U> Left(T left)
		{
			return new Either<T, U>(left);
		}

		public static Either<T, U> Right(U right)
		{
			return new Either<T, U>(right);
		}

		public Either<T, V> Map<V>(Func<U, V> f)
		{
			if (isLeft) {
				return new Either<T, V>(left);
			}

			return new Either<T, V>(f(right));
		}

		public Either<T, V> Bind<V>(Func<U, Either<T, V>> f)
		{
			if (isLeft) {
				return new Either<T, V>(left);
			}

			return f(right);
		}


		public void Do(Action<U> f)
		{
			if (!isLeft) {
				f(right);
			}
		}

		public bool IsLeft()
		{
			return isLeft;
		}

		public bool IsRight()
		{
			return !isLeft;
		}

		public T Left()
		{
			if (!isLeft) {
				throw new EitherAccessException(true);
			}

			return left;
		}

		public U Right()
		{
			if (isLeft) {
				throw new EitherAccessException(false);
			}

			return right;
		}
	}


	public class EitherAccessException : Exception
	{
		public EitherAccessException(bool attemptedLeftAccess)
		  : base(attemptedLeftAccess ?
				"Either expected to be Right, but was Left." :
				"Either expected to be Left, but was Right.")
		{
		}
	}
}
