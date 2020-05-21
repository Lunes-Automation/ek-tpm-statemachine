using System;
using System.Collections.Generic;
using System.Linq;

namespace EK.TPM.Data.Definitions
{
    /// <summary>
    /// Contains the definition of a process step exit point.
    /// </summary>
    public class ExitPointDefinition
    {
        //======================================================
        //      Private fields and constants
        //======================================================

        //======================================================
        //      Constructors and finalizers
        //======================================================

        //======================================================
        //      Public methods
        //======================================================
        /// <summary>
        /// Checks if two exit points are equal, which is the case if they share the same name.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True if both objects are equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ExitPointDefinition e)
                return Name == e.Name;

            return false;
        }

        /// <summary>
        /// Gets the hash code for the exit point, which is the hash code of its name.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }

        //======================================================
        //      Properties
        //======================================================
        /// <summary>
        /// The default exit point for all process steps.
        /// </summary>
        public static readonly ExitPointDefinition Default = new ExitPointDefinition()
        {
            IsError = false,
            Name = "Done"
        };

        /// <summary>
        /// Gets a human readable name for the exit.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets if this exit is invoked when an error occurred inside the step.
        /// </summary>
        public bool IsError
        {
            get;
            set;
        }

        //======================================================
        //      Operators
        //======================================================
        public static bool operator == (ExitPointDefinition a, ExitPointDefinition b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(ExitPointDefinition a, ExitPointDefinition b)
        {
            return !(a == b);
        }

        //======================================================
        //      Private methods
        //======================================================
    }

} // End of namespace EK.TPM.StateMachine
