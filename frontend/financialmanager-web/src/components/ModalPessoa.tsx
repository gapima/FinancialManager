import { ReactNode, useEffect } from "react";
import "./Modal.css";

type Props = {
  open: boolean;
  title: string;
  children: ReactNode;
  onClose: () => void;
  footer?: ReactNode;
};

export default function Modal({ open, title, children, onClose, footer }: Props) {
  useEffect(() => {
    function onKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape") onClose();
    }
    if (open) window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div className="modalOverlay" onMouseDown={onClose}>
      <div className="modal" onMouseDown={(e) => e.stopPropagation()}>
        <div className="modalHeader">
          <h2>{title}</h2>
          <button className="iconBtn" onClick={onClose} aria-label="Fechar">
            ✕
          </button>
        </div>

        <div className="modalBody">{children}</div>

        {footer && <div className="modalFooter">{footer}</div>}
      </div>
    </div>
  );
}
