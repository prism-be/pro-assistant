import { PencilSquareIcon, TrashIcon } from "@heroicons/react/24/outline";
import { useTranslation } from "react-i18next";

interface Props {
    item: any;
    onEdit?: (item: any) => void;
    onDelete?: (item: any) => void;
    onClick?: (item: any) => void;
    title: string;
    selected?: boolean;
}

export const ListItem = ({ item, onEdit, onDelete, onClick, title, selected }: Props) => {
    const { t } = useTranslation("common");

    function deleteItem() {
        if (confirm(t("confirmations.deleteItem") ?? "")) {
            onDelete?.(item);
        }
    }

    return (
        <div className={"flex p-2 mt-1 border" + (selected ? " bg-gray-100" : " border-white")}>
            {onEdit && (
                <a className={"w-6 cursor-pointer"} onClick={() => onEdit(item)}>
                    {" "}
                    <PencilSquareIcon />{" "}
                </a>
            )}
            {onDelete && (
                <a className={"w-6 ml-2 cursor-pointer"} onClick={deleteItem}>
                    {" "}
                    <TrashIcon />{" "}
                </a>
            )}
            <div
                className={"pl-2" + (onclick ? " pointer" : "")}
                onClick={() => {
                    if (onClick) onClick(item);
                }}
            >
                {title}
            </div>
        </div>
    );
};
