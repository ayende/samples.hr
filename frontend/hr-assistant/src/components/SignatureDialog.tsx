import React, { useState, useRef, useEffect } from 'react';
import ReactMarkdown from 'react-markdown';
import './SignatureDialog.css';

interface SignatureDialogProps {
  isOpen: boolean;
  title: string;
  description: string;
  onConfirm: (signature: string) => void;
  onCancel: () => void;
}

export const SignatureDialog: React.FC<SignatureDialogProps> = ({
  isOpen,
  title,
  description,
  onConfirm,
  onCancel
}) => {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [isDrawing, setIsDrawing] = useState(false);
  const [hasSignature, setHasSignature] = useState(false);

  // Dragging state
  const [isDragging, setIsDragging] = useState(false);
  const [dragOffset, setDragOffset] = useState({ x: 0, y: 0 });
  const [position, setPosition] = useState({ x: window.innerWidth / 2, y: window.innerHeight / 2 });

  useEffect(() => {
    if (isOpen) {
      // Reset position to center when dialog opens
      setPosition({ x: window.innerWidth / 2, y: window.innerHeight / 2 });

      if (canvasRef.current) {
        const canvas = canvasRef.current;
        const ctx = canvas.getContext('2d');
        if (ctx) {
          // Set canvas size
          canvas.width = canvas.offsetWidth * 2;
          canvas.height = canvas.offsetHeight * 2;
          ctx.scale(2, 2);

          // Set drawing styles
          ctx.strokeStyle = '#000';
          ctx.lineWidth = 2;
          ctx.lineCap = 'round';
          ctx.lineJoin = 'round';

          // Clear canvas
          ctx.clearRect(0, 0, canvas.width, canvas.height);
          setHasSignature(false);
        }
      }
    }
  }, [isOpen]);

  // Dragging handlers
  const handleMouseDown = (e: React.MouseEvent) => {
    setIsDragging(true);
    setDragOffset({
      x: e.clientX - position.x,
      y: e.clientY - position.y
    });
  };

  useEffect(() => {
    const handleMouseMove = (e: MouseEvent) => {
      if (isDragging) {
        setPosition({
          x: e.clientX - dragOffset.x,
          y: e.clientY - dragOffset.y
        });
      }
    };

    const handleMouseUp = () => {
      setIsDragging(false);
    };

    if (isDragging) {
      document.addEventListener('mousemove', handleMouseMove);
      document.addEventListener('mouseup', handleMouseUp);
    }

    return () => {
      document.removeEventListener('mousemove', handleMouseMove);
      document.removeEventListener('mouseup', handleMouseUp);
    };
  }, [isDragging, dragOffset]);

  const startDrawing = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    setIsDrawing(true);
    draw(e);
  };

  const draw = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    if (!isDrawing || !canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const rect = canvas.getBoundingClientRect();
    let clientX, clientY;

    if ('touches' in e) {
      clientX = e.touches[0].clientX;
      clientY = e.touches[0].clientY;
    } else {
      clientX = e.clientX;
      clientY = e.clientY;
    }

    const x = clientX - rect.left;
    const y = clientY - rect.top;

    ctx.lineTo(x, y);
    ctx.stroke();
    ctx.beginPath();
    ctx.moveTo(x, y);

    setHasSignature(true);
  };

  const stopDrawing = () => {
    if (!isDrawing || !canvasRef.current) return;

    setIsDrawing(false);
    const ctx = canvasRef.current.getContext('2d');
    if (ctx) {
      ctx.beginPath();
    }
  };

  const clearSignature = () => {
    if (!canvasRef.current) return;

    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    if (ctx) {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      setHasSignature(false);
    }
  };

  const handleConfirm = () => {
    if (!canvasRef.current || !hasSignature) return;

    const canvas = canvasRef.current;
    const signatureData = canvas.toDataURL('image/png');
    onConfirm(signatureData);
  };

  const handleOverlayClick = (e: React.MouseEvent) => {
    if (e.target === e.currentTarget) {
      onCancel();
    }
  };

  if (!isOpen) return null;

  return (
    <div
      className={`dialog-overlay ${isOpen ? 'visible' : 'hidden'}`}
      onClick={handleOverlayClick}
    >
      <div
        className="dialog-container"
        style={{ left: position.x, top: position.y }}
      >
        <div className="dialog-header" onMouseDown={handleMouseDown}>
          <h2 className="dialog-title">{title}</h2>
        </div>

        <div className="dialog-content">
          <div className="description">
            <ReactMarkdown>{description}</ReactMarkdown>
          </div>

          <div className="signature-section">
            <label className="signature-label">Your Signature:</label>
            <div className="signature-canvas-container">
              <canvas
                className="signature-canvas"
                ref={canvasRef}
                onMouseDown={startDrawing}
                onMouseMove={draw}
                onMouseUp={stopDrawing}
                onMouseLeave={stopDrawing}
                onTouchStart={startDrawing}
                onTouchMove={draw}
                onTouchEnd={stopDrawing}
                style={{ width: '100%', height: '150px' }}
              />
              {hasSignature && (
                <button className="clear-button" onClick={clearSignature}>
                  Clear
                </button>
              )}
            </div>
            <div className="signature-instructions">
              Sign above with your mouse or finger
              {hasSignature && (
                <>
                  {' • '}
                  <button
                    className="clear-button"
                    onClick={clearSignature}
                    style={{ background: 'none', border: 'none', textDecoration: 'underline' }}
                  >
                    Clear signature
                  </button>
                </>
              )}
            </div>
          </div>
        </div>

        <div className="dialog-actions">
          <button className="cancel-button" onClick={onCancel}>
            Cancel
          </button>
          <button
            className="confirm-button"
            onClick={handleConfirm}
            disabled={!hasSignature}
          >
            Sign & Confirm
          </button>
        </div>
      </div>
    </div>
  );
};
