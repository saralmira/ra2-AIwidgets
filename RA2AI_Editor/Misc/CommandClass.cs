using AIcore.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIcore
{
    public abstract class CommandClass
    {
        public abstract void Do();
        public abstract void Undo();

        public string Name { get; set; }
    }

    public class LayoutCommandClass : CommandClass
    {
        public LayoutCommandClass(OType from, OType to)
        {
            LastObject = from;
            NextObject = to;
        }

        public override void Do()
        {
            RA2AI_Editor.MainWindow.JumpTo(NextObject, false);
        }

        public override void Undo()
        {
            RA2AI_Editor.MainWindow.JumpTo(LastObject, false);
        }

        public OType NextObject { get; protected set; }
        public OType LastObject { get; protected set; }
    }

    public abstract class TypeCommandClass<TList, T> : CommandClass where TList : TLists.TTypeLists<T> where T : Types.OType
    {
        public TypeCommandClass(TList list)
        {
            tlist = list;
        }

        protected readonly TList tlist;
        public T TypeObject { get; protected set; }
        protected int index;
    }

    public abstract class AddTypeCommandClass<TList, T> : TypeCommandClass<TList, T> where TList : TLists.TTypeLists<T> where T : Types.OType
    {
        public AddTypeCommandClass(TList list, int _index) : base(list)
        {
            index = _index + 1;
        }

        public override void Do()
        {
            tlist.Add(TypeObject, index);
            RA2AI_Editor.MainWindow.JumpTo(TypeObject);
        }

        public override void Undo()
        {
            tlist.Delete(TypeObject);
            RA2AI_Editor.MainWindow.InitPageEvent?.Invoke();
        }
    }

    public abstract class DeleteTypeCommandClass<TList, T> : TypeCommandClass<TList, T> where TList : TLists.TTypeLists<T> where T : Types.OType
    {
        public DeleteTypeCommandClass(T del, TList list) : base(list)
        {
            TypeObject = del;
            index = tlist.GetTypeIndex(TypeObject);
        }

        public DeleteTypeCommandClass(string deltag, TList list) : base(list)
        {
            TypeObject = tlist.Find(deltag);
            index = tlist.GetTypeIndex(TypeObject);
        }

        public override void Do()
        {
            tlist.Delete(TypeObject);
            RA2AI_Editor.MainWindow.InitPageEvent?.Invoke();
        }

        public override void Undo()
        {
            tlist.Add(TypeObject, index);
            RA2AI_Editor.MainWindow.JumpTo(TypeObject);
        }
    }

    public class CommandStack : INotifyPropertyChanged
    {
        public readonly ObservableCollection<CommandClass> ExecutedCommand;
        public readonly ObservableCollection<CommandClass> UndoCommand;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CommandStack(int maxStackSize = 1024)
        {
            ExecutedCommand = new ObservableCollection<CommandClass>();
            UndoCommand = new ObservableCollection<CommandClass>();
            IsUndoEnabled = false;
            IsRedoEnabled = false;
            MaxStackSize = maxStackSize;
        }

        public void Clear()
        {
            ExecutedCommand.Clear();
            UndoCommand.Clear();
            IsUndoEnabled = false;
            IsRedoEnabled = false;
        }

        public void Push(CommandClass c)
        {
            ExecutedCommand.Add(c);
            if (ExecutedCommand.Count > MaxStackSize)
                ExecutedCommand.RemoveAt(0);
            if (UndoCommand.Count > 0)
                UndoCommand.Clear();
            c.Do();
            IsUndoEnabled = true;
            IsRedoEnabled = false;
        }

        public void Undo()
        {
            if (ExecutedCommand.Count == 0)
                return;
            CommandClass c = ExecutedCommand[ExecutedCommand.Count - 1];
            ExecutedCommand.RemoveAt(ExecutedCommand.Count - 1);
            UndoCommand.Add(c);
            c.Undo();
            IsUndoEnabled = false;
            IsRedoEnabled = true;
        }

        public void Redo()
        {
            if (UndoCommand.Count == 0)
                return;
            CommandClass c = UndoCommand[UndoCommand.Count - 1];
            UndoCommand.RemoveAt(UndoCommand.Count - 1);
            ExecutedCommand.Add(c);
            c.Do(); 
            IsUndoEnabled = true;
            IsRedoEnabled = false;
        }

        public bool IsRedoEnabled { get { return UndoCommand.Count > 0; } private set { OnPropertyChanged(nameof(IsRedoEnabled)); } }
        public bool IsUndoEnabled { get { return ExecutedCommand.Count > 0; } private set { OnPropertyChanged(nameof(IsUndoEnabled)); } }

        protected int MaxStackSize;
    }
}
