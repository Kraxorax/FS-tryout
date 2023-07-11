module Matrix

open System

let flat2Darray array2D = 
                seq { for x in [0..(Array2D.length1 array2D) - 1] do 
                          for y in [0..(Array2D.length2 array2D) - 1] do 
                              yield array2D.[x, y] }

let upX, upY, upRightX, upRightY, rightX, rightY, rightDownX, rightDownY, downX, downY, downLeftX,downLeftY, leftX, leftY, leftUpX, leftUpY =
    0, 1, 1, 1, 1, 0, 1, -1, 0, -1, -1, -1, -1, 0, -1, 1

type Matrix<'T>(x: int, y: int, generator: (int -> int -> 'T)) =
    do if (x < 2 || x < 2) then raise (ArgumentOutOfRangeException())
    let arr = Array2D.init<'T> x y generator
    let maxX = x - 1
    let maxY = y - 1
    member val X = x
    member val Y = y

    new(x: int, y: int, item: 'T) =
        new Matrix<'T>(x, y, fun _ _ -> item)

    member this.Item
        with get(x: int, y: int) = arr[x, y]
        and set(x: int, y: int) (value:'T) = arr[x, y] <- value

    member this.Map(f: 'T -> 'T): Matrix<'T> =
        new Matrix<'T>(x, y, fun x y -> f arr[x, y])

    member this.MapToArray(f: 'T -> 'G): 'G[] =
        [|  for x in [0..maxX] do
                for y in [0..maxY] do 
                    yield f(arr.[x, y])
        |]


    member this.Neighbours(x: int, y: int): 'T[] =
        if (x < 0 || y < 0 || maxX < x || maxY < y) then raise (ArgumentOutOfRangeException())

        if (0 < x  && 0 < y && x < maxX && y < maxY)
            // Cell is not on border, all 8 neighbours are availabe
            then [| arr[x + upX, y + upY];
                    arr[x + upRightX, y + upRightY]; 
                    arr[x + rightX, y + rightY]; 
                    arr[x + rightDownX, y + rightDownY]; 
                    arr[x + downX, y + downY]; 
                    arr[x + downLeftX, y + downLeftY]; 
                    arr[x + leftX, y + leftY]; 
                    arr[x + leftUpX, y + leftUpY];
            |]
        else if ((x,y) = (0,0)) then [|arr[0, 1]; arr[1, 0]; arr[1, 1]|]
        else if ((x,y) = (0,maxY)) then [|arr[0, maxY - 1]; arr[1, maxY]; arr[1, maxY - 1]|]
        else if ((x,y) = (maxX, maxY)) then [|arr[maxX - 1, maxY - 1]; arr[maxX, maxY - 1]; arr[maxX - 1, maxY]|]
        else if ((x,y) = (maxX, 0)) then [|arr[maxX - 1, 0]; arr[maxX, 1]; arr[maxX - 1, 1];|]
        else if (x = 0) then [|arr[0, y + 1]; arr[1, y + 1]; arr[1, y]; arr[1, y - 1]; arr[0, y - 1] |]
        else if (x = maxX) then [|arr[maxX, y + 1]; arr[maxX, y - 1]; arr[maxX - 1, y - 1]; arr[maxX - 1, y]; arr[maxX - 1, y + 1] |]
        else if (y = 0) then [|arr[x + 1, 0]; arr[x + 1, 1]; arr[x, 1]; arr[x - 1, 1]; arr[x - 1, 0] |]
        else if (y = maxY) then [|arr[x, maxY - 1]; arr[x + 1, maxY - 1]; arr[x + 1, maxY]; arr[x - 1, maxY]; arr[x - 1, maxY - 1] |]
        else [||]