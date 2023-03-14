export function formatAmount(value: number | string)
{
    if (value as string)
    {
        value = parseFloat(value as string);
    }
    
    return (value as number).toFixed(2);
}