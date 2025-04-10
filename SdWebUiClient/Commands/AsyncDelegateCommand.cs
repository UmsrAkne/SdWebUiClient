using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SdWebUiClient.Commands
{
    public class AsyncDelegateCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        ///     非同期メソッドの入力をサポートする DelegateCommand です。
        /// </summary>
        /// <param name="execute">実行するメソッド。 await を使用可能。</param>
        /// <param name="canExecute">実行可否を返すメソッド。デフォルトは null であり、省略可能。</param>
        /// <exception cref="ArgumentNullException"> execute が null の場合にスローされます。</exception>
        public AsyncDelegateCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private async Task ExecuteAsync()
        {
            await execute();
        }
    }
}