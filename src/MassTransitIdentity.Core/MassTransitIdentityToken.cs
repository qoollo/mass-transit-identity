namespace MassTransitIdentity.Core
{
    public class MassTransitIdentityToken
    {
        private Guid? _value;

        public Guid? Value
        {
            get => _value;
            set
            {
                if (_value is null || _value == Guid.Empty)
                {
                    _value = value;
                }
                else
                {
                    throw new InvalidOperationException($"{nameof(MassTransitIdentityToken)} is already set to {_value}.");
                }
            }
        }
    }
}