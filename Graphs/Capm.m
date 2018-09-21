% Draw 3 isoutility functions
utilsOffset = -.893;		% Ajustado a ojo =)
x=0:.2:10;
util1=.1*x.^2+.1*x+5+utilsOffset;
util2=.15*x.^2+.1*x+6.5+utilsOffset;
util3=.2*x.^2+.1*x+8+utilsOffset;

% Parametros de la frontera de eficiencia
efA = .3;
efB = -1.5;
efC = 3;

eficienty = 1.4:.2:10;
eficientx = efA*eficienty.^2 + efB*eficienty + efC;

% Para efPtoX, la front de eficiencia en Y vale=
efPtoX = 5;
efPtoY = (-efB + (efB^2 - 4*efA*(efC-efPtoX)) ^ .5) / (2*efA);
pteCml = 1/(2*efA*efPtoY + efB);
ordOrigCml = efPtoY - pteCml * efPtoX;

cmly = ordOrigCml + pteCml * x;

plot(x, util1, '-r;Curvas de Isoutilidad;',...
	x, util2, '-r;;', ...
	x, util3, '-r;;',...
	eficientx, eficienty, '-b;Frontera de Eficiencia;',...
	x, cmly, '-k;Capital Allocation Line;');

axis("labelxy");
xlabel('Riesgo');
ylabel("Rentabilidad");
%title("Utilidad y Frontera de Eficiencia");
axis([0,10,0,10,0,10],"autoz","ticz");
grid("off");


% SAVE THE FILES
print -color -dashes -dpng "capm.png";




% Create a plot only with the CAL
plot(eficientx, eficienty, '-b;Frontera de Eficiencia;');

axis("labelxy");
xlabel('Riesgo');
ylabel("Rentabilidad");
%title("Utilidad y Frontera de Eficiencia");
axis([0,10,0,10,0,10],"autoz","ticz");
grid("off");

print -color -dashes -dpng "capm-fronef-only.png";

