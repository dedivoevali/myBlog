import "../FlagEmoji/FlagEmoji.scss";
import { FlagEmojiProps } from "./flag-emoji-props";

export const FlagEmoji = ({ emoji, fontSizePx = 12 }: FlagEmojiProps) =>
{
    return (
        <div style={{ fontSize: `${fontSizePx}px` }} className="flag-emoji-content">
            {emoji || ' '}
        </div>
    )
}