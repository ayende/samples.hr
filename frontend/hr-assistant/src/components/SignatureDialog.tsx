import React, { useState, useRef, useEffect } from 'react';
import styled from 'styled-components';
import ReactMarkdown from 'react-markdown';
import { theme } from '../theme';

interface SignatureDialogProps {
    isOpen: boolean;
    title: string;
    description: string;
    onConfirm: (signature: string) => void;
    onCancel: () => void;
}

const DialogOverlay = styled.div<{ isOpen: boolean }>`
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: ${props => props.isOpen ? 'block' : 'none'};
  z-index: 1000;
  backdrop-filter: blur(0.5px);
`;

const DialogContainer = styled.div<{ $x: number; $y: number }>`
  background: white;
  border-radius: ${theme.borderRadius.large};
  box-shadow: ${theme.shadows.large};
  max-width: 600px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
  position: fixed;
  left: ${props => props.$x}px;
  top: ${props => props.$y}px;
  transform: translate(-50%, -50%);
  animation: slideUp 0.3s ease-out;
  
  @keyframes slideUp {
    from {
      transform: translate(-50%, -50%) translateY(20px);
      opacity: 0;
    }
    to {
      transform: translate(-50%, -50%) translateY(0);
      opacity: 1;
    }
  }
`;

const DialogHeader = styled.div`
  background: linear-gradient(135deg, ${theme.colors.primary.main}, ${theme.colors.secondary.main});
  color: white;
  padding: ${theme.spacing.lg};
  border-radius: ${theme.borderRadius.large} ${theme.borderRadius.large} 0 0;
  cursor: move;
  user-select: none;
  
  &:active {
    cursor: grabbing;
  }
`;

const DialogTitle = styled.h2`
  margin: 0;
  font-size: ${theme.typography.h3.fontSize};
  font-weight: ${theme.typography.h3.fontWeight};
`;

const DialogContent = styled.div`
  padding: ${theme.spacing.lg};
`;

const Description = styled.div`
  margin-bottom: ${theme.spacing.lg};
  line-height: ${theme.typography.body.lineHeight};
  color: ${theme.colors.text.primary};
  max-height: 400px;
  overflow-y: auto;
  padding-right: ${theme.spacing.sm};
  
  /* Custom scrollbar styling */
  &::-webkit-scrollbar {
    width: 6px;
  }
  
  &::-webkit-scrollbar-track {
    background: ${theme.colors.background.secondary};
    border-radius: ${theme.borderRadius.small};
  }
  
  &::-webkit-scrollbar-thumb {
    background: ${theme.colors.border.medium};
    border-radius: ${theme.borderRadius.small};
  }
  
  &::-webkit-scrollbar-thumb:hover {
    background: ${theme.colors.primary.light};
  }
  
  h1, h2, h3, h4, h5, h6 {
    margin: ${theme.spacing.md} 0 ${theme.spacing.sm};
    color: ${theme.colors.primary.main};
  }
  
  p {
    margin: ${theme.spacing.sm} 0;
  }
  
  ul, ol {
    margin: ${theme.spacing.sm} 0;
    padding-left: ${theme.spacing.lg};
  }
  
  li {
    margin: ${theme.spacing.xs} 0;
  }
  
  strong {
    font-weight: 600;
    color: ${theme.colors.primary.main};
  }
  
  em {
    font-style: italic;
    opacity: 0.9;
  }
  
  code {
    background: ${theme.colors.background.accent};
    padding: 2px 4px;
    border-radius: ${theme.borderRadius.small};
    font-family: 'Courier New', monospace;
  }
  
  blockquote {
    border-left: 4px solid ${theme.colors.primary.light};
    margin: ${theme.spacing.md} 0;
    padding-left: ${theme.spacing.md};
    background: ${theme.colors.background.accent};
    border-radius: 0 ${theme.borderRadius.small} ${theme.borderRadius.small} 0;
  }
`;

const SignatureSection = styled.div`
  margin-bottom: ${theme.spacing.lg};
`;

const SignatureLabel = styled.label`
  display: block;
  font-weight: 600;
  color: ${theme.colors.text.primary};
  margin-bottom: ${theme.spacing.sm};
`;

const SignatureCanvas = styled.canvas`
  border: 2px solid ${theme.colors.border.medium};
  border-radius: ${theme.borderRadius.medium};
  cursor: crosshair;
  background: white;
  width: 100%;
  height: 150px;
  touch-action: none;
  
  &:focus {
    outline: none;
    border-color: ${theme.colors.primary.main};
    box-shadow: 0 0 0 3px ${theme.colors.primary.lighter};
  }
`;

const SignatureInstructions = styled.div`
  font-size: ${theme.typography.small.fontSize};
  color: ${theme.colors.text.secondary};
  margin-top: ${theme.spacing.xs};
  text-align: center;
`;

const DialogActions = styled.div`
  display: flex;
  gap: ${theme.spacing.md};
  justify-content: flex-end;
  padding: 0 ${theme.spacing.lg} ${theme.spacing.lg};
`;

const Button = styled.button<{ variant?: 'primary' | 'secondary' }>`
  padding: ${theme.spacing.md} ${theme.spacing.xl};
  border-radius: ${theme.borderRadius.medium};
  font-weight: 600;
  cursor: pointer;
  transition: all ${theme.transitions.normal};
  border: none;
  
  ${props => props.variant === 'primary' ? `
    background: linear-gradient(135deg, ${theme.colors.primary.main}, ${theme.colors.primary.dark});
    color: white;
    
    &:hover:not(:disabled) {
      transform: translateY(-1px);
      box-shadow: ${theme.shadows.medium};
    }
  ` : `
    background: ${theme.colors.background.secondary};
    color: ${theme.colors.text.primary};
    border: 1px solid ${theme.colors.border.medium};
    
    &:hover:not(:disabled) {
      background: ${theme.colors.background.accent};
    }
  `}
  
  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }
`;

const ClearButton = styled.button`
  background: none;
  border: none;
  color: ${theme.colors.text.secondary};
  cursor: pointer;
  font-size: ${theme.typography.small.fontSize};
  text-decoration: underline;
  margin-top: ${theme.spacing.xs};
  
  &:hover {
    color: ${theme.colors.primary.main};
  }
`;

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
        <DialogOverlay isOpen={isOpen} onClick={handleOverlayClick}>
            <DialogContainer $x={position.x} $y={position.y}>
                <DialogHeader onMouseDown={handleMouseDown}>
                    <DialogTitle>{title}</DialogTitle>
                </DialogHeader>

                <DialogContent>
                    <Description>
                        <ReactMarkdown>{description}</ReactMarkdown>
                    </Description>

                    <SignatureSection>
                        <SignatureLabel>Your Signature:</SignatureLabel>
                        <SignatureCanvas
                            ref={canvasRef}
                            onMouseDown={startDrawing}
                            onMouseMove={draw}
                            onMouseUp={stopDrawing}
                            onMouseLeave={stopDrawing}
                            onTouchStart={startDrawing}
                            onTouchMove={draw}
                            onTouchEnd={stopDrawing}
                        />
                        <SignatureInstructions>
                            Sign above with your mouse or finger
                            {hasSignature && (
                                <>
                                    {' • '}
                                    <ClearButton onClick={clearSignature}>Clear signature</ClearButton>
                                </>
                            )}
                        </SignatureInstructions>
                    </SignatureSection>
                </DialogContent>

                <DialogActions>
                    <Button variant="secondary" onClick={onCancel}>
                        Cancel
                    </Button>
                    <Button
                        variant="primary"
                        onClick={handleConfirm}
                        disabled={!hasSignature}
                    >
                        Sign & Confirm
                    </Button>
                </DialogActions>
            </DialogContainer>
        </DialogOverlay>
    );
};
