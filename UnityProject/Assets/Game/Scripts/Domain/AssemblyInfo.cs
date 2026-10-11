using System.Runtime.CompilerServices;

// GameSession の状態を変更するメソッドは internal にしてあり、呼び出せるのは UseCase 層だけにしている。
// Presentation から直接変更されないことを、取り決めではなくコンパイラで保証するため。
[assembly: InternalsVisibleTo("Orquvia.Starter.UseCase")]
[assembly: InternalsVisibleTo("Orquvia.Starter.Tests.EditMode")]
