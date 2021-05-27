namespace OctoAwesome.OctoMath
{
    public class Polynomial
    {
        private readonly float[] _coefficients;

        public Polynomial(params float[] coefficients)
        {
            _coefficients = coefficients;
        }

        public float Evaluate(float px)
        {
            if (_coefficients.Length == 0)
                return 0;

            var result = _coefficients[0];
            var x = px;

            for (var i = 1; i < _coefficients.Length; ++i)
            {
                result += x * _coefficients[i];
                x *= px;
            }

            return result;
        }
    }
}