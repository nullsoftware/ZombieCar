using System.Threading;
using Cysharp.Threading.Tasks;

namespace ZombieCar.Controls
{
    public interface ITapInput
    {
        /// <summary>
        /// Completes on the next new tap/click. A press that was already consumed never completes it twice.
        /// </summary>
        UniTask WaitForTapAsync(CancellationToken cancellationToken);
    }
}
