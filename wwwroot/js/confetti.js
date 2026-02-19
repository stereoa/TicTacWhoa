window.startConfetti = () => {
    if (typeof confetti === 'undefined') {
        console.warn('Confetti library not loaded');
        return;
    }

    const duration = 3000;
    const end = Date.now() + duration;

    (function frame() {
        // Launch a few confetti from the left edge
        confetti({
            particleCount: 5,
            angle: 60,
            spread: 55,
            origin: { x: 0 }
        });

        // and a few from the right edge
        confetti({
            particleCount: 5,
            angle: 120,
            spread: 55,
            origin: { x: 1 }
        });

        if (Date.now() < end) {
            requestAnimationFrame(frame);
        }
    }());
};
