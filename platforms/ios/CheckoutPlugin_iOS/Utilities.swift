public typealias CStringPtr = UnsafePointer<CChar>

public typealias VoidDelegate = @convention(c) () -> Void;
public typealias StringDelegate = @convention(c) (CStringPtr) -> Void;
public typealias IntDelegate = @convention(c) (Int32) -> Void;
public typealias LongDelegate = @convention(c) (Int64) -> Void;
public typealias FloatDelegate = @convention(c) (Float) -> Void;
public typealias BoolDelegate = @convention(c) (Bool) -> Void;

public typealias BoolStringDelegate = @convention(c) (Bool, CStringPtr) -> Void;

extension UnsafePointer where Pointee == CChar {
    func toString() -> String { String(cString: self) }
}
