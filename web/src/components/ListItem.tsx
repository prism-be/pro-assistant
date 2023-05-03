import { PencilSquareIcon, TrashIcon } from "@heroicons/react/24/outline";

interface Props {
    item: any;
    onEdit?: (item: any) => void;
    onDelete?: (item: any) => void;
    title: string;
}

export const ListItem = ({ item, onEdit, onDelete, title }: Props) => {
    return <div className={"flex"}>
        {onEdit && <a className={"w-6 cursor-pointer"} onClick={() => onEdit(item)}>
            {" "}
            <PencilSquareIcon />{" "}
        </a>}
        {onDelete && <a className={"w-6 ml-2 cursor-pointer"} onClick={() => onDelete(item)}>
            {" "}
            <TrashIcon />{" "}
        </a>}
        <div className={"pl-2"}>{title}</div>
    </div>
}