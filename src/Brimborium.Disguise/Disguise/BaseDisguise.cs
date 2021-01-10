using System;

namespace Brimborium.Disguise {
    public abstract class BaseDisguise {
        private ContextDisguise? _ContextDisguise;

        protected BaseDisguise(ContextDisguise? contextDisguise) {
            this._ContextDisguise = contextDisguise;
        }

        public ContextDisguise? ContextDisguise {
            get {
                return this._ContextDisguise;
            }
            set {
                if (ReferenceEquals(this._ContextDisguise, value)) {
                    return;
                } else if (this._ContextDisguise is object) {
                    throw new InvalidOperationException("");
                } else {
                    this._ContextDisguise = value;
                    this.ContextDisguiseUpdated();
                }
            }
        }

        protected virtual void ContextDisguiseUpdated() { }
    }
}
