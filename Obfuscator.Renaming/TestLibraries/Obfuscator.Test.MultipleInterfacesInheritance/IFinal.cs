
namespace Obfuscator.Test.MultipleInterfacesInheritance
{
    public interface IFinal : IMiddle, ISameAsBaseB
    {
        void FinalMethod();

        // NOTE explicit interface declaration can only be declared in a class or struct
        // void IBaseA.BaseAMethod();
    }
}
