﻿namespace Shay.Payment.Notify.Events
{
    public class PaymentUnknowEventArgs : PaymentEventArgs
    {
        public PaymentUnknowEventArgs(DGateway gateway)
            : base(gateway)
        {
        }
    }
}
